#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Domain.Invitations;

public record InvitationCreated(EntityId Id, Email Receiver, DateTime ExpiresAtUtc) : IDomainEvent;