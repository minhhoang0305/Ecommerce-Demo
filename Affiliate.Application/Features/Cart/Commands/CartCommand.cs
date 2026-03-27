using MediatR;

public record CartCommand(Guid UserId) : IRequest<Cart?>;
