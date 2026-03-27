using MediatR;

public record CreateOrderCommand(List<(string ProductName, decimal Price, int Quantity)>Items) : IRequest<Guid>;