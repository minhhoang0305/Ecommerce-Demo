public class CartItem
{
    public Guid Id {get; set;}
    public Guid CartId {get; set;}
    public Guid ProductId {get; set;}
    public string Name {get; set;} = default!;
    public int Quantity {get; set;}
}