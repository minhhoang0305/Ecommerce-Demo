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
            trackedCart.UserId = cart.UserId;
            trackedCart.AppliedCouponCode = cart.AppliedCouponCode;
            trackedCart.AppliedDiscount = cart.AppliedDiscount;

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

    public async Task ClearAsync(Cart cart)
    {
        _context.CartItems.RemoveRange(cart.Items);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();
    }
}
