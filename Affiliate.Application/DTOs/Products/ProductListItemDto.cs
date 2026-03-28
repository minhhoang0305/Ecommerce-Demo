namespace Affiliate.Application.DTOs.Products;

public record ProductListItemDto(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    string Description,
    int Stock,
    int Status);
