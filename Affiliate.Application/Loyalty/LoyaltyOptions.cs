namespace Affiliate.Application.Loyalty;

public class LoyaltyOptions
{
    public const string SectionName = "Loyalty";

    // 100,000 VND -> 1 point
    public decimal VndPerPoint { get; set; } = 100_000m;

    // Ranking thresholds based on total accumulated points.
    public int GoldFromPoints { get; set; } = 100;
    public int DiamondFromPoints { get; set; } = 500;

    // Extra discount (%) applied per order based on current rank.
    public decimal SilverDiscountPercent { get; set; } = 0m;
    public decimal GoldDiscountPercent { get; set; } = 2m;
    public decimal DiamondDiscountPercent { get; set; } = 5m;
}

