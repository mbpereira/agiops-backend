using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record InvitationAccepted(Guid Token, DateTime AcceptedAtUtc) : IDomainEvent;
}