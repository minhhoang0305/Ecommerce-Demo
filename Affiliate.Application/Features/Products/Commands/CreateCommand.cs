using MediatR;

public record CreateCommand(string Name, string Category, decimal Price, string Description, int Stock) : IRequest<Guid>;
