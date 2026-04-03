using MediatR;

public record ApplyCouponToCartCommand(Guid UserId, string Code) : IRequest<CartCouponResult>;

