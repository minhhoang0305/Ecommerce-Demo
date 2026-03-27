using System.Security.Claims;
using Affiliate.Application.DTOs;
using MediatR;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/cart", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            var cart = await mediator.Send(new CartCommand(userId));
            return cart is null ? Results.NotFound() : Results.Ok(cart);
        }).RequireAuthorization();

        app.MapPut("api/v1/cart/items/{id:guid}", async (
            Guid id,
            CartQuantityRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            await mediator.Send(new UpdateCartItemCommand(id, request.Quantity, userId));
            return Results.Ok(new { message = "Cart item updated successfully" });
        }).RequireAuthorization();

        app.MapPost("api/v1/cart/add", async (
            CartAddItemRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            if (!TryGetUserId(user, out var userId))
                return Results.Unauthorized();

            await mediator.Send(new AddToCartCommand(request.ProductId, request.Quantity, userId));
            return Results.Ok(new { message = "Item added to cart successfully" });
        }).RequireAuthorization();
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out userId);
    }
}
