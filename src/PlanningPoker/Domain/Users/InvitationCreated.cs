using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record InvitationCreated(Guid Token, Email To, DateTime ExpiresAtUtc) : IDomainEvent;
}
