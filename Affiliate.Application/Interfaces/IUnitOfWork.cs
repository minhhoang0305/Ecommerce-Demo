using System.Data;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}
