using MediatR;
public static class OrderEndpoint
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/orders", async (
            CreateOrderCommand command,
            IMediator mediator) =>
        {
            var userId = await mediator.Send(command);
            return Results.Ok();
        }
        );
        app.MapPost("api/v1/orders/checkout", async (
            CheckoutCommand command,
            IMediator mediator) =>
        {
            var userId = await mediator.Send(command);
            return Results.Ok("Đặt hàng thành công");
        }
        );
    }
}