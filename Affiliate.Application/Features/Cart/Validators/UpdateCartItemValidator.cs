using FluentValidation;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.CartItemId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
