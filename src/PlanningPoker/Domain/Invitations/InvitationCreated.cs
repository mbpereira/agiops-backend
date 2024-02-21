using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Domain.Invitations
{
    public record InvitationCreated(Guid Token, Email Receiver, DateTime ExpiresAtUtc) : IDomainEvent;
}