public class Orders
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public List<OrderItems> Items { get; private set; } = new();
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    public bool IsPaid { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void AddItem(string productName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

        Items.Add(new OrderItems(productName, price, quantity));
    }

    public void MarkAsPaid()
    {
        IsPaid = true;
    }

}
