using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users.Events
{
    public record InvitationCreated(Guid Token, Email To, DateTime ExpiresAtUtc) : IDomainEvent;
}
