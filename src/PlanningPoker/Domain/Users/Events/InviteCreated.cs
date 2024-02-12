using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users.Events
{
    public record InviteCreated(Guid Token, Email To, DateTime ExpiresAtUtc) : IDomainEvent;
}
