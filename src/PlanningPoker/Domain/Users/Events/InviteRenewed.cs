using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users.Events
{
    public record InviteRenewed(Guid Token, Email To, DateTime ExpiresAtUtc) : IDomainEvent;
}
