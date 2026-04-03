using MediatR;
using System.Security.Claims;
using Affiliate.Application.DTOs.Reviews;
using Affiliate.Domain.Entities;
public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/products/{productId:guid}/reviews", async (
            Guid productId,
            int? take,
            IMediator mediator) =>
        {
            var result = await mediator.Send(
                new GetReviewCommand(productId, take));

            return Results.Ok(result);
        }).WithTags("Review");

        app.MapPost("/api/v1/reviews", async (
            CreateReviewRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var id = await mediator.Send(new CreateReviewCommand(
                userId,
                request.ProductId,
                request.Rating,
                request.Comment));
            return Results.Ok(id);
        }).RequireAuthorization("UserOnly").WithTags("Review");
    }
}
