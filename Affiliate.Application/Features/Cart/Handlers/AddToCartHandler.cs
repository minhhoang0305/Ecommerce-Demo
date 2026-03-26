using MediatR;
public class AddToCartHandler : IRequestHandler<AddToCartCommand, Unit>
{
    private readonly IProductRepository _productRepo;
    private readonly ICartRepository _cartRepo;

    public AddToCartHandler(IProductRepository productRepo, ICartRepository cartRepo)
    {
        _productRepo = productRepo;
        _cartRepo = cartRepo;
    }

    public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // 1. Check product tồn tại
        var product = await _productRepo.GetByIdAsync(request.ProductId);

        if (product == null || product.IsDeleted)
            throw new Exception("Product not found");

        // 2. Check tồn kho
        if (product.Stock < request.Quantity)
            throw new Exception("Not enough stock");

        // 3. Lấy cart user
        var cart = await _cartRepo.GetByUserIdAsync(request.UserId);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId
            };
        }

        // 4. Check item đã tồn tại chưa
        var existingItem = cart.Items
            .FirstOrDefault(x => x.ProductId == request.ProductId);

        if (existingItem != null)
        {
            // cộng thêm quantity
            if (product.Stock < existingItem.Quantity + request.Quantity)
                throw new Exception("Not enough stock");

            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }

        await _cartRepo.SaveAsync(cart);

        return Unit.Value;
    }
}