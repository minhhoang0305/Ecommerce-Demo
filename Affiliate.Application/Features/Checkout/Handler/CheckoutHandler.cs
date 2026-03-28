using Affiliate.Application.DTOs;
using MediatR;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, OrderDTO>
{
    private readonly IOrderRepository _orderRepository;

    public CheckoutHandler(IOrderRepository orderReponsitory)
    {
        _orderRepository = orderReponsitory;
    }

    public async Task<OrderDTO> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.CheckoutAsync(request.UserId, request.PaymentMethod, cancellationToken);

        return new OrderDTO(
            order.Id,
            order.Items.Select(x => new OrderItemDTO(x.ProductName, x.Price, x.Quantity)).ToList(),
            order.TotalAmount,
            order.IsPaid,
            order.CreatedAt);
    }
}
