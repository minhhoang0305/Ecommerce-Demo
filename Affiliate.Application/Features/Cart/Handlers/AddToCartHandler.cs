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
        var product = await _productRepo.GetByIdAsync(request.ProductId);

        if (product == null || product.IsDeleted)
            throw new Exception("Product not found");

        if (product.Stock < request.Quantity)
            throw new Exception("Not enough stock");

        var cart = await _cartRepo.GetByUserIdAsync(request.UserId);

        if (cart == null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Items = new List<CartItem>()
            };
        }

        var existingItem = cart.Items
            .FirstOrDefault(x => x.ProductId == request.ProductId);

        if (existingItem != null)
        {
            if (product.Stock < existingItem.Quantity + request.Quantity)
                throw new Exception("Not enough stock");

            existingItem.Quantity += request.Quantity;
            existingItem.Price = product.Price;
            existingItem.Name = product.Name;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Quantity = request.Quantity,
                Name = product.Name,
                Price = product.Price
            });
        }

        await _cartRepo.SaveAsync(cart);

        return Unit.Value;
    }
}