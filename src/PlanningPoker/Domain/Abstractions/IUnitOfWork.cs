using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IGamesRepository Games { get; }
        IIssuesRepository Issues { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
