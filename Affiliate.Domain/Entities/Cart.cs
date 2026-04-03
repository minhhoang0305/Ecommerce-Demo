public class Cart
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public List<CartItem> Items {get; set;} = new List<CartItem>();
    public string? AppliedCouponCode { get; set; }
    public decimal AppliedDiscount { get; set; }
}
