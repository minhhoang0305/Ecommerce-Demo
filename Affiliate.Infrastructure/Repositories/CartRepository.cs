using Microsoft.EntityFrameworkCore;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task SaveAsync(Cart cart)
    {
        var trackedCart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cart.Id);

        if (trackedCart == null)
        {
            await _context.Carts.AddAsync(cart);
        }
        else
        {
            // update scalar
            trackedCart.UserId = cart.UserId;

            // sync items
            foreach (var item in cart.Items)
            {
                var existingItem = trackedCart.Items
                    .FirstOrDefault(x => x.Id == item.Id);

                if (existingItem == null)
                {
                    trackedCart.Items.Add(new CartItem
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity
                    });
                }
                else
                {
                    existingItem.Quantity = item.Quantity;
                    existingItem.Price = item.Price;
                    existingItem.Name = item.Name;
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
