namespace Affiliate.Application.DTOs.Products;

public class ProductsRequest
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
}