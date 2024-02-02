using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IGameRepository Games { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
