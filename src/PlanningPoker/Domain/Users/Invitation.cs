using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class InvitationConstants
    {
        public const int ExpirationTimeInMinutes = 30;

        public static class Messages
        {
            public const string AlreadyAcceptedInvitation = "This invitation has already been accepted or is inactive.";
            public const string ExpiredInvitation = "This invitation has expired.";
        }
    }

    public class Invitation : AggregateRoot<Invitation>, ITenantable
    {

        public EntityId TenantId { get; private set; }
        public Guid Token { get; private set; }
        public Role Role { get; private set; }
        public Email To { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime SentAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public InvitationStatus Status { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; } = null;
        public bool IsOpen => InvitationStatus.Open.Equals(Status);
        public bool HasExpired => DateTime.UtcNow > ExpiresAtUtc;

        private Invitation(EntityId id, EntityId tenantId, Email to, Role role)
            : this(id, tenantId, to, role,
                  token: Guid.NewGuid(),
                  createdAtUtc: DateTime.UtcNow,
                  sentAtUtc: DateTime.UtcNow,
                  expiresAtUtc: DateTime.UtcNow.AddMinutes(InvitationConstants.ExpirationTimeInMinutes),
                  status: InvitationStatus.Open)
        {
            RaiseDomainEvent(new InvitationCreated(Token, To, ExpiresAtUtc));
        }

        public Invitation(EntityId id, EntityId tenantId, Email to, Role role, Guid token, DateTime createdAtUtc, DateTime sentAtUtc, DateTime expiresAtUtc, InvitationStatus status, DateTime? updatedAtUtc = null)
            : base(id)
        {
            TenantId = tenantId;
            Token = token;
            Role = role;
            To = to;
            CreatedAtUtc = createdAtUtc;
            SentAtUtc = sentAtUtc;
            ExpiresAtUtc = expiresAtUtc;
            Status = status;
            UpdatedAtUtc = updatedAtUtc;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<Invitation> validator)
        {
            validator.CreateRuleFor(i => i.To.Value, nameof(To))
                .NotEmpty()
                .NotNull()
                .EmailAddress();
        }

        public void Renew()
        {
            if (!IsOpen)
            {
                AddError(nameof(Renew), InvitationConstants.Messages.AlreadyAcceptedInvitation);
                return;
            }

            SentAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = SentAtUtc.AddMinutes(InvitationConstants.ExpirationTimeInMinutes);
            RaiseDomainEvent(new InvitationRenewed(Token, To, ExpiresAtUtc));
        }

        public void Accept()
        {
            if (!IsOpen)
            {
                AddError(nameof(Accept), InvitationConstants.Messages.AlreadyAcceptedInvitation);
                return;
            }

            if (HasExpired)
            {
                AddError(nameof(Accept), InvitationConstants.Messages.ExpiredInvitation);
                return;
            }

            UpdatedAtUtc = DateTime.UtcNow;
            Status = InvitationStatus.Accepted;
            RaiseDomainEvent(new InvitationAccepted(Token, UpdatedAtUtc.GetValueOrDefault()));
        }

        public static Invitation New(int tenantId, string to, Role role) =>
            new(EntityId.AutoIncrement(), new EntityId(tenantId), new Email(to), role);

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
                new(new EntityId(id),
                    new EntityId(tenantId),
                    new Email(to),
                    role,
                    token,
                    createdAtUtc,
                    sentAtUtc,
                    expiresAtUtc,
                    status,
                    updatedAtUtc);
    }
}
