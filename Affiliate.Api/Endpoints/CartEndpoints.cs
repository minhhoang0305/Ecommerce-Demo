using MediatR;
public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/cart/add", async (AddToCartCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok();
        });
    }
}