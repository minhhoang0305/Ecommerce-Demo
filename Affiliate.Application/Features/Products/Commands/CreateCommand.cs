using MediatR;

public record CreateCommand(string Name, decimal Price, string Description, int Stock) :IRequest<Guid>;
