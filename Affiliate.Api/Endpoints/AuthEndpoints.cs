using MediatR;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/register", async (
            RegisterUserCommand command,
            IMediator mediator) =>
        {
            var userId = await mediator.Send(command);
            return Results.Ok(new { userId });
        }).WithTags("Auth");

        app.MapPost("/api/v1/auth/login", async (
            LoginUserCommand command,
            IMediator mediator) =>
        {
            var authResponse = await mediator.Send(command);
            return Results.Ok(authResponse);
        }).WithTags("Auth");
    }
}