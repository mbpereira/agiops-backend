using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;

namespace PlanningPoker.Domain.Users
{
    public class Invitation : TenantableAggregateRoot
    {
        public Guid Token { get; private set; }
        public Role Role { get; private set; }
        public Email Receiver { get; private set; } = Email.Empty();
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime SentAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public InvitationStatus Status { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; } = null;
        public bool IsOpen => InvitationStatus.Sent.Equals(Status);
        public bool HasExpired => DateTime.UtcNow > ExpiresAtUtc;

        private Invitation(int id, int tenantId, string to, Role role)
            : this(id, tenantId, to, role,
                  token: Guid.NewGuid(),
                  createdAtUtc: DateTime.UtcNow,
                  sentAtUtc: DateTime.UtcNow,
                  expiresAtUtc: DateTime.UtcNow.AddMinutes(InvitationConstants.ExpirationTimeInMinutes),
                  status: InvitationStatus.Sent)
        {
            RaiseDomainEvent(new InvitationCreated(Token, Receiver, ExpiresAtUtc));
        }

        public Invitation(
            int id,
            int tenantId,
            string receiver,
            Role role,
            Guid token,
            DateTime createdAtUtc,
            DateTime sentAtUtc,
            DateTime expiresAtUtc,
            InvitationStatus status,
            DateTime? updatedAtUtc = null)
            : base(id, tenantId)
        {
            Token = token;
            Role = role;
            SetReceiver(receiver);
            CreatedAtUtc = createdAtUtc;
            SentAtUtc = sentAtUtc;
            ExpiresAtUtc = expiresAtUtc;
            Status = status;
            UpdatedAtUtc = updatedAtUtc;
        }

        public void SetReceiver(string email)
        {
            if (!email.IsEmail())
            {
                AddError(InvitationErrors.InvalidReceiver);
                return;
            }

            Receiver = new Email(email);
        }

        public void Renew()
        {
            if (!IsOpen)
            {
                AddError(InvitationErrors.AlreadyAcceptedInvitation);
                return;
            }

            SentAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = SentAtUtc.AddMinutes(InvitationConstants.ExpirationTimeInMinutes);
            RaiseDomainEvent(new InvitationRenewed(Token, Receiver, ExpiresAtUtc));
        }

        public void Accept()
        {
            if (!IsOpen)
            {
                AddError(InvitationErrors.AlreadyAcceptedInvitation);
                return;
            }

            if (HasExpired)
            {
                AddError(InvitationErrors.ExpiredInvitation);
                return;
            }

            UpdatedAtUtc = DateTime.UtcNow;
            Status = InvitationStatus.Accepted;
            RaiseDomainEvent(new InvitationAccepted(Token, UpdatedAtUtc.GetValueOrDefault()));
        }

        public static Invitation New(int tenantId, string to, Role role) =>
            new(EntityId.AutoIncrement(), tenantId, to, role);

        public static Invitation Load(
            int id,
            int tenantId,
            string to,
            Role role,
            Guid token,
            DateTime createdAtUtc,
            DateTime sentAtUtc,
            DateTime expiresAtUtc,
            InvitationStatus status,
            DateTime? updatedAtUtc = null) =>
                new(id,
                    tenantId,
                    to,
                    role,
                    token,
                    createdAtUtc,
                    sentAtUtc,
                    expiresAtUtc,
                    status,
                    updatedAtUtc);
    }
}
