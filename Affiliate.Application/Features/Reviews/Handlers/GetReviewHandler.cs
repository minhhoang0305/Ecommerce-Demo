using Affiliate.Application.DTOs.Reviews;
using MediatR;

public class GetReviewHandler : IRequestHandler<GetReviewCommand, ProductReviewsDto>
{
    private readonly IReviewRepository _reviewRepository;
    public GetReviewHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }
    public async Task<ProductReviewsDto> Handle(GetReviewCommand request, CancellationToken cancellationToken)
    {
        var productId = request.productId;
        var take = request.Take ?? 20;
        take = Math.Clamp(take, 1, 20);

        var reviews = await _reviewRepository.GetByProductIdAsync(productId, take);

            var total = reviews.Count;
            var average = total == 0 ? 0m : Math.Round((decimal)reviews.Sum(x => x.Rating) / total, 2, MidpointRounding.AwayFromZero);

        var dto = new ProductReviewsDto(
            productId,
            average,
            total,
            reviews.Select(x => new ReviewItemDto(
            x.Id,
            x.UserId,
            x.Rating,
            x.Comment,
            x.CreatedAt)).ToList());

        return dto;
    }
}