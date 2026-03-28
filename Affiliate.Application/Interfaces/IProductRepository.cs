public interface IProductRepository
{
    Task<Guid> CreateAsync(Products product);
    Task<Products?> GetByIdAsync(Guid id);
    Task<IEnumerable<Products>> GetAllAsync();
    Task<(IReadOnlyList<Products> Items, int TotalCount)> GetPagedAsync(
        int page,
        int size,
        string? category,
        decimal? minPrice,
        decimal? maxPrice);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Products product);
}
