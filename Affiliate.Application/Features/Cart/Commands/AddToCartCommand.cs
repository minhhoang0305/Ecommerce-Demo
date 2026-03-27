using MediatR;

public record AddToCartCommand(Guid ProductId, int Quantity, Guid UserId) : IRequest<Unit>;
