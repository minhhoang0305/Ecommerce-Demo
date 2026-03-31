using Affiliate.Domain.Entities;

public interface IOrderRepository
{
    Task<Orders?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Orders>> GetByUserIdAsync(Guid userId);
    Task<Orders> CheckoutAsync(Guid userId, string paymentMethod, string? couponCode, CancellationToken cancellationToken);
    Task<Orders> CreatePendingVnPayOrderAsync(Guid userId, string? couponCode, CancellationToken cancellationToken);
    Task FinalizePendingVnPayOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task DeletePendingOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateAsync(Orders order);
    Task SaveAsync(Orders order);
}
