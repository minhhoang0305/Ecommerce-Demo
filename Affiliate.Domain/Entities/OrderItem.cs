public class OrderItems
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public Orders Order { get; private set; } = null!;

    private OrderItems()
    {
    }

    public OrderItems(string productName, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

        Id = Guid.NewGuid();
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }
}
