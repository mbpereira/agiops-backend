using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users.Events
{
    public record InvitationRenewed(Guid Token, Email To, DateTime ExpiresAtUtc) : IDomainEvent;
}
