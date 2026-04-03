using System.Security.Claims;
using Affiliate.Application.DTOs;
using Affiliate.Application.DTOs.Coupons;
using MediatR;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/cart", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var cart = await mediator.Send(new CartCommand(userId));
            return cart is null ? Results.Ok("Empty") : Results.Ok(cart);
        }).RequireAuthorization("UserOnly").WithTags("Cart");

        app.MapPut("api/v1/cart/items/{id:guid}", async (
            Guid id,
            CartQuantityRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            await mediator.Send(new UpdateCartItemCommand(id, request.Quantity, userId));
            return Results.Ok(new { message = "Cart item updated successfully" });
        }).RequireAuthorization("UserOnly").WithTags("Cart");

        app.MapPost("api/v1/cart/add", async (
            CartAddItemRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            await mediator.Send(new AddToCartCommand(request.ProductId, request.Quantity, userId));
            return Results.Ok(new { message = "Item added to cart successfully" });
        }).RequireAuthorization("UserOnly").WithTags("Cart");

        app.MapPost("api/v1/cart/apply-coupon", async (
            ApplyCouponToCartRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
        if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var result = await mediator.Send(new ApplyCouponToCartCommand(userId, request.Code));
            return Results.Ok(result);
        }).RequireAuthorization("UserOnly").WithTags("Cart");
    }
}
