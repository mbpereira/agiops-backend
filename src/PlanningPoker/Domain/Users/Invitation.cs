using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users.Events;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public class Invitation : AggregateRoot<Invitation>, ITenantable
    {
        public const int ExpirationTimeInMinutes = 30;

        public EntityId TenantId { get; private set; }
        public Guid Token { get; private set; }
        public Role Role { get; private set; }
        public Email To { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime SentAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public InvitationStatus Status { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; } = null;

        private Invitation(EntityId id, EntityId tenantId, Email to, Role role)
            : this(id, tenantId, to, role,
                  token: Guid.NewGuid(),
                  createdAtUtc: DateTime.UtcNow,
                  sentAtUtc: DateTime.UtcNow,
                  expiresAtUtc: DateTime.UtcNow.AddMinutes(ExpirationTimeInMinutes),
                  status: InvitationStatus.Open)
        {
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
            SentAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = SentAtUtc.AddMinutes(ExpirationTimeInMinutes);
            RaiseDomainEvent(new InvitationRenewed(Token, To, ExpiresAtUtc));
        }

        public void Accept()
        {
            if (Status != InvitationStatus.Open)
                throw new DomainException("This invitation has already been accepted or is inactive.");

            if (DateTime.UtcNow > ExpiresAtUtc)
                throw new DomainException("This invitation has expired.");

            UpdatedAtUtc = DateTime.UtcNow;
            Status = InvitationStatus.Accepted;
        }

        public static Invitation New(int tenantId, string to, Role role)
        {
            var invitation = new Invitation(EntityId.AutoIncrement(), new EntityId(tenantId), new Email(to), role);
            invitation.RaiseDomainEvent(new InvitationCreated(invitation.Token, invitation.To, invitation.ExpiresAtUtc));
            return invitation;
        }

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
