using MediatR;

public record CheckoutCommand(Guid OrderId, string PaymentMethod) : IRequest<bool>;