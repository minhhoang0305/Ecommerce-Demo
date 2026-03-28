using Affiliate.Application.DTOs;
using MediatR;

public record CheckoutCommand(Guid UserId, string PaymentMethod) : IRequest<OrderDTO>;
