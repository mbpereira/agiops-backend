using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record InvitationCreated(Guid Token, Email Receiver, DateTime ExpiresAtUtc) : IDomainEvent;
}
