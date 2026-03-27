using Microsoft.EntityFrameworkCore;
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task SaveAsync(Orders order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Orders?> GetByIdAsync(Guid id)
    {
        return await _context.Orders.FindAsync(id);
    }
}