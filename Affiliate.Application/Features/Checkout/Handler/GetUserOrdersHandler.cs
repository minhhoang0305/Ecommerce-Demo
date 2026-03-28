using Affiliate.Application.DTOs;
using MediatR;

public class GetUserOrdersHandler : IRequestHandler<GetUserOrdersQuery, IReadOnlyList<OrderDTO>>
{
    private readonly IOrderRepository _orderRepository;

    public GetUserOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDTO>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByUserIdAsync(request.UserId);

        return orders
            .Select(order => new OrderDTO(
                order.Id,
                order.Items.Select(item => new OrderItemDTO(item.ProductName, item.Price, item.Quantity)).ToList(),
                order.TotalAmount,
                order.IsPaid,
                order.CreatedAt))
            .ToList();
    }
}
