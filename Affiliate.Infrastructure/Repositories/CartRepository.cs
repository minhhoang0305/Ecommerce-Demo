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
        var existingCart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cart.Id);

        if (existingCart == null)
        {
            await _context.Carts.AddAsync(cart);
        }
        else
        {
            // update items
            _context.Entry(existingCart).CurrentValues.SetValues(cart);

            // xử lý item
            foreach (var item in cart.Items)
            {
                var existingItem = existingCart.Items
                    .FirstOrDefault(x => x.Id == item.Id);

                if (existingItem == null)
                {
                    existingCart.Items.Add(item);
                }
                else
                {
                    existingItem.Quantity = item.Quantity;
                }
            }
        }

        await _context.SaveChangesAsync(); 
    }
}