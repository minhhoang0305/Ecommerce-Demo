public record CartCouponResult(
    string Code,
    decimal CartTotal,
    decimal Discount,
    decimal FinalTotal
);
