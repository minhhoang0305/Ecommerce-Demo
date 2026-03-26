using MediatR;
public record UpdateCommand(Guid Id, string Name, decimal Price, string Description) : IRequest<Unit>;