using PlanningPoker.Domain.Issues;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IGamesRepository Games { get; }
        IIssuesRepository Issues { get; }
        ITenantsRepository Tenants { get; }
        IAccessGrantsRepository AccessGrants { get; }
        IInvitationsRepository Invitations { get; }
        IVotingSystemsRepository VotingSystems { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}