using MediatR;

public record UpdateCommand(Guid Id, string Name, string Category, decimal Price, string Description, int Stock) : IRequest<Unit>;
