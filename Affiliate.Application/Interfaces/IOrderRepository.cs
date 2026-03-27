using Affiliate.Domain.Entities;
public interface IOrderRepository
{
    Task<Orders?> GetByIdAsync(Guid id);
    Task UpdateAsync(Orders order);
    Task SaveAsync(Orders order);

}