using Affiliate.Application.DTOs.Common;
using Affiliate.Application.DTOs.Products;
using MediatR;

public record GetProductsQuery(
    int Page = 1,
    int Size = 10,
    string? Category = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null) : IRequest<PagedResult<ProductListItemDto>>;
