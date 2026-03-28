public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task SaveAsync(Cart cart);
    Task ClearAsync(Cart cart);
}
