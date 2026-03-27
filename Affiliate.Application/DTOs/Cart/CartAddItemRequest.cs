namespace Affiliate.Application.DTOs;

public class CartAddItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
