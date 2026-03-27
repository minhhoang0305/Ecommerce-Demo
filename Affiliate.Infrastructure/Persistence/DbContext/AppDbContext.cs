using Microsoft.EntityFrameworkCore;
using Affiliate.Domain.Entities;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Products> Products => Set<Products>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Orders> Orders => Set<Orders>();
    public DbSet<OrderItems> OrderItems => Set<OrderItems>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}