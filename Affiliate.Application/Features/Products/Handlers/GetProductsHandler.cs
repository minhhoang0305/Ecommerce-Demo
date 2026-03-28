using Affiliate.Application.DTOs.Common;
using Affiliate.Application.DTOs.Products;
using MediatR;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IProductRepository _repository;

    public GetProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var size = request.Size <= 0 ? 10 : Math.Min(request.Size, 100);

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            size,
            request.Category,
            request.MinPrice,
            request.MaxPrice);

        return new PagedResult<ProductListItemDto>(
            items.Select(x => new ProductListItemDto(
                x.Id,
                x.Name,
                x.Category,
                x.Price,
                x.Description,
                x.Stock,
                x.Status)).ToList(),
            page,
            size,
            totalCount);
    }
}
