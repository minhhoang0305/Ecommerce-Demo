using MediatR;
public record DeleteCommand(Guid Id) : IRequest<Unit>;
