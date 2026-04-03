using MediatR;

public class CreateCouponHandler : IRequestHandler<CreateCouponCommand, Guid>
{
    private readonly ICouponRepository _couponRepository;

    public CreateCouponHandler(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var existingCoupon = await _couponRepository.GetByCodeAsync(request.Code);
        if (existingCoupon != null)
            throw new ArgumentException("Coupon code already exists");

        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Code = request.Code.Trim(),
            DiscountType = request.DiscountType,
            Value = request.Value,
            MinOrderValue = request.MinOrderValue,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            UsageLimit = request.UsageLimit,
            TimesUsed = 0
        };

        return await _couponRepository.CreateAsync(coupon);
    }
}
