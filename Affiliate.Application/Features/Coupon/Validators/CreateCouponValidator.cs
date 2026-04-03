using Affiliate.Domain.Entities;
using FluentValidation;

public class CreateCouponValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.MinOrderValue)
            .GreaterThanOrEqualTo(0);

        When(x => x.DiscountType == Affiliate.Domain.Entities.DiscountType.PercentTag,() => 
        {
            RuleFor(x => x.Value)
                .GreaterThan(0)
                .LessThanOrEqualTo(100);
        });

        When(x => x.DiscountType == Affiliate.Domain.Entities.DiscountType.FixedAmount, ()=>
        {
            RuleFor(x => x.Value)
                .GreaterThan(0);
        });

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate);

        RuleFor(x => x.UsageLimit)
            .GreaterThanOrEqualTo(0);
    }
}
