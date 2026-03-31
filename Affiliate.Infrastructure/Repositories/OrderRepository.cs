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
        return await _context.Orders
            .Include(x => x.Items)
            .Include(x => x.Coupon)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Orders>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .Include(x => x.Coupon)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Orders> CheckoutAsync(Guid userId, string paymentMethod, string? couponCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new Exception("Payment method is required");
        if (string.Equals(paymentMethod, "VNPAY", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Use VNPay pending payment flow");

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

        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var coupon = await _context.Coupon.FirstOrDefaultAsync(
                x => x.Code == couponCode.Trim(),
                cancellationToken);

            if (coupon == null)
                throw new Exception("Coupon not found");

            order.ApplyCoupon(coupon);
            coupon.TimesUsed++;
        }

        if (!string.Equals(paymentMethod, "VNPAY", StringComparison.OrdinalIgnoreCase))
        {
            order.MarkAsPaid();
        }

        await _context.Orders.AddAsync(order, cancellationToken);
        _context.CartItems.RemoveRange(cart.Items);
        _context.Carts.Remove(cart);

        await _context.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task UpdateAsync(Orders order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Orders> CreatePendingVnPayOrderAsync(Guid userId, string? couponCode, CancellationToken cancellationToken)
    {
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

            order.AddItem(product.Name, product.Price, cartItem.Quantity);
        }

        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var coupon = await _context.Coupon.FirstOrDefaultAsync(
                x => x.Code == couponCode.Trim(),
                cancellationToken);

            if (coupon == null)
                throw new Exception("Coupon not found");

            order.ApplyCoupon(coupon);
        }

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task FinalizePendingVnPayOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(x => x.Items)
            .Include(x => x.Coupon)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        if (order == null)
            throw new Exception("Order not found");

        if (order.IsPaid)
            return;

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == order.UserId, cancellationToken);

        if (cart == null || cart.Items.Count == 0)
            throw new Exception("Cart is empty");

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
        }

        if (order.CouponId.HasValue)
        {
            var coupon = await _context.Coupon.FirstOrDefaultAsync(x => x.Id == order.CouponId.Value, cancellationToken);
            if (coupon == null)
                throw new Exception("Coupon not found");

            coupon.TimesUsed++;
        }

        order.MarkAsPaid();
        _context.CartItems.RemoveRange(cart.Items);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePendingOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        if (order == null || order.IsPaid)
            return;

        _context.OrderItems.RemoveRange(order.Items);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
