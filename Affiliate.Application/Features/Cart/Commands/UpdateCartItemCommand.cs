using MediatR;

public record UpdateCartItemCommand(Guid CartItemId, int Quantity, Guid UserId) : IRequest<Unit>;
