using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Domain.Users
{
    public record InvitationRenewed(Guid Token, Email Receiver, DateTime NewExpirationDateUtc) : IDomainEvent;
}