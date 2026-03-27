using MediatR;

public class CartHandler : IRequestHandler<CartCommand, Cart?>
{
    private readonly ICartRepository _cartRepo;

    public CartHandler(ICartRepository cartRepo)
    {
        _cartRepo = cartRepo;
    }

    public async Task<Cart?> Handle(CartCommand request, CancellationToken cancellationToken)
    {
        return await _cartRepo.GetByUserIdAsync(request.UserId);
    }
}
