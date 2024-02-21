using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Invitations
{
    public record InvitationAccepted(Guid Token, DateTime AcceptedAtUtc) : IDomainEvent;
}