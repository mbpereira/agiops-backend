#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Domain.Invitations;

public record InvitationRenewed(EntityId Id, Email Receiver, DateTime NewExpirationDateUtc) : IDomainEvent;