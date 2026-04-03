using Affiliate.Application.DTOs.Reviews;
using MediatR;

public record GetReviewCommand(
    Guid productId, 
    int? Take
) : IRequest<ProductReviewsDto>;