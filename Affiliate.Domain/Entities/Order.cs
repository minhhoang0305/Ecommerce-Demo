using System.Data.Common;
using Affiliate.Domain.Entities;

public class Orders
{
    public const string StatusCreated = "CREATED";
    public const string StatusPaid = "PAID";
    public const string StatusCompleted = "COMPLETED";

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public List<OrderItems> Items { get; private set; } = new();
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    public decimal Discount {get; private set;}
    public decimal FinalAmount => TotalAmount - Discount;
    public bool IsPaid { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string Status { get; private set; } = StatusCreated;
    public DateTime? CompletedAt { get; private set; }
    public int LoyaltyPointsAwarded { get; private set; }
    public decimal RankDiscount { get; private set; }

    public Guid? CouponId {get; private set;}
    public Coupon? Coupon {get; private set;}


    public void AddItem(Guid ProductId, string productName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

        Items.Add(new OrderItems(ProductId, productName, price, quantity));
    }

    public void ApplyCoupon(Coupon coupon)
    {
        if (coupon == null || !coupon.IsActive)
            throw new ArgumentException("Coupon don't active");
        if (DateTime.UtcNow < coupon.StartDate || DateTime.UtcNow > coupon.EndDate)
            throw new ArgumentException("Coupon expirydate");
        if (coupon.UsageLimit > 0 && coupon.TimesUsed >= coupon.UsageLimit)
            throw new ArgumentException("Coupon đã được sử dụng");
        
        if (TotalAmount < coupon.MinOrderValue)
            throw new ArgumentException("Order total is not enough for this coupon");

        Discount = coupon.DiscountType switch
        {
            DiscountType.PercentTag => Math.Round(TotalAmount * (coupon.Value / 100m), 2, MidpointRounding.AwayFromZero),
            DiscountType.FixedAmount => Math.Round(Math.Min(coupon.Value, TotalAmount), 2, MidpointRounding.AwayFromZero),
            _ => 0m
        };

        if (Discount < 0)
            Discount = 0;
        if (Discount > TotalAmount)
            Discount = TotalAmount;
        CouponId = coupon.Id;
        Coupon = coupon;
    }

    public void MarkAsPaid()
    {
        IsPaid = true;
        if (!string.Equals(Status, StatusCompleted, StringComparison.OrdinalIgnoreCase))
            Status = StatusPaid;
    }

    // Replaces any existing rank discount to keep this operation idempotent.
    public void SetRankDiscountPercent(decimal percent)
    {
        if (percent < 0 || percent > 100)
            throw new ArgumentOutOfRangeException(nameof(percent), "Rank discount percent must be between 0 and 100.");

        var newRankDiscount = Math.Round(TotalAmount * (percent / 100), 2, MidpointRounding.AwayFromZero);
        var newTotalDiscount = (Discount - RankDiscount) + newRankDiscount;

        if (newTotalDiscount < 0)
            newTotalDiscount = 0;
        if (newTotalDiscount > TotalAmount)
            newTotalDiscount = TotalAmount;

        RankDiscount = newRankDiscount;
        Discount = Math.Round(newTotalDiscount, 2, MidpointRounding.AwayFromZero);
    }

    public void MarkAsCompleted(int pointsAwarded)
    {
        if (!IsPaid)
            throw new InvalidOperationException("Only paid orders can be completed.");

        if (string.Equals(Status, StatusCompleted, StringComparison.OrdinalIgnoreCase))
            return;

        Status = StatusCompleted;
        CompletedAt = DateTime.UtcNow;
        LoyaltyPointsAwarded = Math.Max(0, pointsAwarded);
    }
}
