using System.Security.Claims;
using Affiliate.Application.DTOs;
using Affiliate.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

public static class OrderEndpoint
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/orders", async (
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            var orders = await mediator.Send(new GetUserOrdersQuery(userId));
            return Results.Ok(orders);
        }).RequireAuthorization("UserOnly");

        app.MapPost("api/v1/orders/checkout", async (
            CheckoutRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            var order = await mediator.Send(new CheckoutCommand(userId, request.PaymentMethod, request.CouponCode));
            return Results.Ok(order);
        }).RequireAuthorization("UserOnly");

        app.MapPost("api/v1/payments/vnpay/create", async (
            CheckoutRequest request,
            HttpContext httpContext,
            ClaimsPrincipal user,
            IOrderRepository orderRepository,
            IVnPayService vnPayService) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            var order = await orderRepository.CreatePendingVnPayOrderAsync(userId, request.CouponCode, httpContext.RequestAborted);
            var paymentUrl = vnPayService.CreatePaymentUrl(new VnPayPaymentRequest(
                order.Id,
                order.FinalAmount,
                $"Thanh toan don hang {order.Id:N}",
                BuildAbsoluteUrl(httpContext, "/api/v1/payments/vnpay/return"),
                GetClientIpAddress(httpContext)));

            return Results.Ok(new
            {
                orderId = order.Id,
                amount = order.FinalAmount,
                paymentUrl
            });
        }).RequireAuthorization("UserOnly");

        app.MapGet("api/v1/payments/vnpay/return", async (
            HttpContext httpContext,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IVnPayService vnPayService) =>
        {
            var validation = vnPayService.ValidateCallback(ToDictionary(httpContext.Request.Query));
            var redirectUrl = BuildPaymentResultRedirect(httpContext, validation);

            if (!validation.IsValidSignature || string.IsNullOrWhiteSpace(validation.OrderReference))
                return Results.Redirect(redirectUrl);

            if (Guid.TryParseExact(validation.OrderReference, "N", out var orderId))
            {
                var order = await orderRepository.GetByIdAsync(orderId);
                if (order is not null && validation.IsSuccess && !order.IsPaid && AreSameAmount(order.FinalAmount, validation.Amount))
                {
                    await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, httpContext.RequestAborted);
                    try
                    {
                        await orderRepository.FinalizePendingVnPayOrderAsync(orderId, httpContext.RequestAborted);
                        await unitOfWork.CommitTransactionAsync(httpContext.RequestAborted);
                    }
                    catch
                    {
                        await unitOfWork.RollbackTransactionAsync(httpContext.RequestAborted);
                        throw;
                    }
                }
                else if (order is not null && !order.IsPaid)
                {
                    await orderRepository.DeletePendingOrderAsync(orderId, httpContext.RequestAborted);
                }
            }

            return Results.Redirect(redirectUrl);
        });

        app.MapGet("api/v1/payments/vnpay/ipn", async (
            HttpContext httpContext,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IVnPayService vnPayService) =>
        {
            var validation = vnPayService.ValidateCallback(ToDictionary(httpContext.Request.Query));
            if (!validation.IsValidSignature || string.IsNullOrWhiteSpace(validation.OrderReference))
            {
                return Results.Json(new { RspCode = "97", Message = "Invalid signature" });
            }

            if (!Guid.TryParseExact(validation.OrderReference, "N", out var orderId))
            {
                return Results.Json(new { RspCode = "01", Message = "Order not found" });
            }

            var order = await orderRepository.GetByIdAsync(orderId);
            if (order is null)
            {
                return Results.Json(new { RspCode = "01", Message = "Order not found" });
            }

            if (!AreSameAmount(order.FinalAmount, validation.Amount))
            {
                return Results.Json(new { RspCode = "04", Message = "Invalid amount" });
            }

            if (validation.IsSuccess && !order.IsPaid)
            {
                await unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, httpContext.RequestAborted);
                try
                {
                    await orderRepository.FinalizePendingVnPayOrderAsync(orderId, httpContext.RequestAborted);
                    await unitOfWork.CommitTransactionAsync(httpContext.RequestAborted);
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync(httpContext.RequestAborted);
                    throw;
                }
            }
            else if (!validation.IsSuccess && !order.IsPaid)
            {
                await orderRepository.DeletePendingOrderAsync(orderId, httpContext.RequestAborted);
            }

            return Results.Json(new { RspCode = "00", Message = "Confirm Success" });
        });
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out userId);
    }

    private static string BuildAbsoluteUrl(HttpContext context, string path)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}{path}";
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";
    }

    private static string BuildPaymentResultRedirect(HttpContext context, VnPayCallbackResult validation)
    {
        var baseUrl = BuildAbsoluteUrl(context, "/");
        return QueryHelpers.AddQueryString(baseUrl, new Dictionary<string, string?>
        {
            ["paymentStatus"] = validation.IsSuccess ? "success" : "failed",
            ["orderRef"] = validation.OrderReference,
            ["responseCode"] = validation.ResponseCode
        });
    }

    private static bool AreSameAmount(decimal left, decimal right)
    {
        return Math.Round(left, 2, MidpointRounding.AwayFromZero) ==
               Math.Round(right, 2, MidpointRounding.AwayFromZero);
    }

    private static IReadOnlyDictionary<string, string> ToDictionary(IQueryCollection query)
    {
        return query.ToDictionary(x => x.Key, x => x.Value.ToString(), StringComparer.Ordinal);
    }
}
