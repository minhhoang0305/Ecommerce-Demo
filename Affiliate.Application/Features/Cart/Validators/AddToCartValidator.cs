using FluentValidation;

public class AddToCartValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
