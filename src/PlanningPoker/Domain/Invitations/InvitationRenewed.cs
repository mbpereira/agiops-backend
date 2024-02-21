using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Domain.Invitations
{
    public record InvitationRenewed(Guid Token, Email Receiver, DateTime NewExpirationDateUtc) : IDomainEvent;
}