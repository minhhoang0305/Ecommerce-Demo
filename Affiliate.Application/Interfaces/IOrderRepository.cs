using Affiliate.Domain.Entities;

public interface IOrderRepository
{
    Task<Orders?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Orders>> GetByUserIdAsync(Guid userId);
    Task<Orders> CheckoutAsync(Guid userId, string paymentMethod, CancellationToken cancellationToken);
    Task UpdateAsync(Orders order);
    Task SaveAsync(Orders order);
}
