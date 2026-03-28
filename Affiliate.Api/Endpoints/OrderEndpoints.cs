using System.Security.Claims;
using Affiliate.Application.DTOs;
using MediatR;

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
        }).RequireAuthorization();

        app.MapPost("api/v1/orders/checkout", async (
            CheckoutRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            var order = await mediator.Send(new CheckoutCommand(userId, request.PaymentMethod));
            return Results.Ok(order);
        }).RequireAuthorization();
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out userId);
    }
}
