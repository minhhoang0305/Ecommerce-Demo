using MediatR;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    public CheckoutHandler(IOrderRepository orderReponsitory)
    {
        _orderRepository = orderReponsitory;
    }
    public async Task<bool> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var order = new GetByIdAsync(request.OrderId);
        if (order == null)
            return false;
        var paymentSuccess = await _paymentGateway.ProcessPayment(Orders.TotalAmount, request.PaymentMethod);
            if (paymentSuccess)
            {
                Orders.MarkAsPaid();
                await _orderRepository.UpdateAsync(order);
                return true;
            }
            return false;
    }
}