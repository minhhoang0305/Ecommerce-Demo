using MediatR;

public class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, Unit>
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;

    public UpdateCartItemHandler(ICartRepository cartRepo, IProductRepository productRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
    }

    public async Task<Unit> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepo.GetByUserIdAsync(request.UserId);
        if (cart == null)
            throw new Exception("Cart not found");

        var cartItem = cart.Items.FirstOrDefault(x => x.Id == request.CartItemId);
        if (cartItem == null)
            throw new Exception("Cart item not found");

        var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
        if (product == null || product.IsDeleted)
            throw new Exception("Product not found");

        if (product.Stock < request.Quantity)
            throw new Exception("Not enough stock");

        cartItem.Quantity = request.Quantity;
        cartItem.Price = product.Price;
        cartItem.Name = product.Name;

        await _cartRepo.SaveAsync(cart);

        return Unit.Value;
    }
}
