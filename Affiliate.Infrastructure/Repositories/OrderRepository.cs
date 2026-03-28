using Microsoft.EntityFrameworkCore;
using System.Data;

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
        return await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Orders>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Orders> CheckoutAsync(Guid userId, string paymentMethod, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new Exception("Payment method is required");

        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart == null || cart.Items.Count == 0)
            throw new Exception("Cart is empty");

        var order = new Orders
        {
            UserId = userId
        };

        foreach (var cartItem in cart.Items)
        {
            var product = await _context.Products
                .FromSqlInterpolated($"SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {cartItem.ProductId}")
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null || product.IsDeleted)
                throw new Exception($"Product {cartItem.ProductId} not found");

            if (product.Stock < cartItem.Quantity)
                throw new Exception($"Product {product.Name} does not have enough stock");

            product.Stock -= cartItem.Quantity;
            order.AddItem(product.Name, product.Price, cartItem.Quantity);
        }

        order.MarkAsPaid();

        await _context.Orders.AddAsync(order, cancellationToken);
        _context.CartItems.RemoveRange(cart.Items);
        _context.Carts.Remove(cart);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return order;
    }

    public async Task UpdateAsync(Orders order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
