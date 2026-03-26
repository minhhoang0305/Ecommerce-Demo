using MediatR;

public record AddToCartCommand(Guid UserId,Guid ProductId, string Name, int Quantity) : IRequest<Unit>;