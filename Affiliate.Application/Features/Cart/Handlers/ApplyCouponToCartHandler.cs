using Affiliate.Domain.Entities;
using MediatR;

public class ApplyCouponToCartHandler : IRequestHandler<ApplyCouponToCartCommand, CartCouponResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;

    public ApplyCouponToCartHandler(ICartRepository cartRepository, ICouponRepository couponRepository)
    {
        _cartRepository = cartRepository;
        _couponRepository = couponRepository;
    }

    public async Task<CartCouponResult> Handle(ApplyCouponToCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(request.UserId);
        if (cart == null || cart.Items.Count == 0)
            throw new Exception("Cart is empty");

        var cartTotal = cart.Items.Sum(x => x.Price * x.Quantity);

        var coupon = await _couponRepository.GetByCodeAsync(request.Code.Trim());
        if (coupon == null)
            throw new Exception("Coupon not found");

        if (!coupon.IsActive)
            throw new Exception("Coupon inactive");

        var now = DateTime.UtcNow;
        if (now < coupon.StartDate || now > coupon.EndDate)
            throw new Exception("Coupon expired");

        if (coupon.UsageLimit > 0 && coupon.TimesUsed >= coupon.UsageLimit)
            throw new Exception("Coupon out of uses");

        if (cartTotal < coupon.MinOrderValue)
            throw new Exception("Cart total is not enough for this coupon");

        var discount = coupon.DiscountType switch
        {
            DiscountType.PercentTag => Math.Round(cartTotal * (coupon.Value / 100m), 2, MidpointRounding.AwayFromZero),
            DiscountType.FixedAmount => Math.Round(Math.Min(coupon.Value, cartTotal), 2, MidpointRounding.AwayFromZero),
            _ => 0m
        };

        if (discount < 0) discount = 0;
        if (discount > cartTotal) discount = cartTotal;

        cart.AppliedCouponCode = coupon.Code;
        cart.AppliedDiscount = discount;
        await _cartRepository.SaveAsync(cart);

        var finalTotal = Math.Max(0, cartTotal - discount);
        return new CartCouponResult(coupon.Code, cartTotal, discount, finalTotal);
    }
}

