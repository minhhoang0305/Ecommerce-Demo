public interface IProductRepository
{
    Task<Guid> CreateAsync(Products product);
    Task<Products?> GetByIdAsync(Guid id);
    Task<IEnumerable<Products>> GetAllAsync();
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Products product);

}