public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task SaveAsync(Cart cart);
}
