using MediatR;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Orders();



        foreach (var item in request.Items)
            order.AddItem(item.ProductName, item.Price, item.Quantity);

        await _orderRepository.SaveAsync(order);
        return order.Id;
    }
}
