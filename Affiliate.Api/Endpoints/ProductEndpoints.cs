using MediatR;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/product/create", async (
            CreateCommand command,
            IMediator mediator) =>
        {
            var productId = await mediator.Send(command);
            return Results.Ok(new { productId, message = "Product created successfully" });
        }).RequireAuthorization("AdminOnly");

        app.MapGet("/api/v1/product/{id}", async (
            Guid id,
            IMediator mediator) =>
        {
            var product = await mediator.Send(new GetByIdAsync(id));
            if (product == null)
                return Results.NotFound(new { message = "Product not found" });

            return Results.Ok(product);
        });

        app.MapGet("/api/v1/products", async (
            int? page,
            int? size,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            IMediator mediator) =>
        {
            var products = await mediator.Send(new GetProductsQuery(page ?? 1, size ?? 10, category, minPrice, maxPrice));
            return Results.Ok(products);
        });

        app.MapDelete("/api/v1/product/{id}", async (
            Guid id,
            IMediator mediator) =>
        {
            await mediator.Send(new DeleteCommand(id));
            return Results.Ok(new { message = "Product deleted successfully" });
        }).RequireAuthorization("AdminOnly");

        app.MapPut("/api/v1/product/update", async (
            UpdateCommand command,
            IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Product updated successfully" });
        }).RequireAuthorization("AdminOnly");
    }
}
