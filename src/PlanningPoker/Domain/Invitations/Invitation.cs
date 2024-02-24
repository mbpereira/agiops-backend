#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Domain.Invitations;

public class Invitation : TenantableAggregateRoot
{
    private Invitation(string id, string tenantId, string to, Role role, IDateTimeProvider dateTimeProvider)
        : base(id, tenantId, dateTimeProvider)
    {
        Role = role;
        SetReceiver(to);
        SentAtUtc = CreatedAtUtc;
        ExpiresAtUtc = SentAtUtc.AddMinutes(InvitationConstants.ExpirationTimeInMinutes);
        Status = InvitationStatus.Sent;
        RaiseDomainEvent(new InvitationCreated(Id, Receiver, ExpiresAtUtc));
    }

    public Role Role { get; private set; }
    public Email Receiver { get; private set; } = Email.Empty();
    public DateTime SentAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public InvitationStatus Status { get; private set; }
    public bool IsOpen => InvitationStatus.Sent.Equals(Status);
    public bool HasExpired => DateTime.UtcNow > ExpiresAtUtc;

    private void SetReceiver(string email)
    {
        if (!email.IsEmail())
        {
            AddError(InvitationErrors.InvalidReceiver);
            return;
        }

        Receiver = new Email(email);
    }

    public void ChangeReceiver(string email)
    {
        SetReceiver(email);
        Updated();
    }

    public void Renew()
    {
        if (!IsOpen)
        {
            AddError(InvitationErrors.FinishedInvitation(nameof(Renew)));
            return;
        }

        SentAtUtc = DateTimeProvider.UtcNow();
        ExpiresAtUtc = SentAtUtc.AddMinutes(InvitationConstants.ExpirationTimeInMinutes);
        Updated();
        RaiseDomainEvent(new InvitationRenewed(Id, Receiver, ExpiresAtUtc));
    }

    public void Cancel()
    {
        if (!IsOpen)
        {
            AddError(InvitationErrors.FinishedInvitation(nameof(Cancel)));
            return;
        }

        Status = InvitationStatus.Cancelled;
        Updated();
    }

    public void Accept()
    {
        if (!IsOpen)
        {
            AddError(InvitationErrors.FinishedInvitation(nameof(Accept)));
            return;
        }

        if (HasExpired)
        {
            AddError(InvitationErrors.ExpiredInvitation);
            return;
        }

        Status = InvitationStatus.Accepted;
        Updated();
        RaiseDomainEvent(new InvitationAccepted(Id, UpdatedAtUtc.GetValueOrDefault()));
    }

    public static Invitation New(string tenantId, string to, Role role, IDateTimeProvider? dateTimeProvider = null)
    {
        return new Invitation(EntityId.Generate(), tenantId, to, role,
            dateTimeProvider ?? DefaultDateTimeProvider.Instance);
    }
}