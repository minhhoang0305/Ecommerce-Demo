using Microsoft.EntityFrameworkCore;
using Affiliate.Domain.Entities;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(Products product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    public async Task<Products?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Products>> GetAllAsync()
    {
        return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
    }

    public async Task<(IReadOnlyList<Products> Items, int TotalCount)> GetPagedAsync(
        int page,
        int size,
        string? category,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = _context.Products.Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        product.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Products product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.Stock = product.Stock;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
        }
    }
}
