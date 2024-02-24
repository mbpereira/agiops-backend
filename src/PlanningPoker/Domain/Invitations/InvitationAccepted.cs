#region

using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.Domain.Invitations;

public record InvitationAccepted(EntityId Id, DateTime AcceptedAtUtc) : IDomainEvent;