using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users.Events;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public class Invite : AggregateRoot<Invite>, ITenantable
    {
        public const int ExpirationTimeInMinutes = 30;

        public EntityId TenantId { get; private set; }
        public Guid Token { get; private set; }
        public Role Role { get; private set; }
        public Email To { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime SentAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public InviteStatus Status { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; } = null;

        private Invite(EntityId id, EntityId tenantId, Email to, Role role)
            : this(id, tenantId, to, role,
                  token: Guid.NewGuid(),
                  createdAtUtc: DateTime.UtcNow,
                  sentAtUtc: DateTime.UtcNow,
                  expiresAtUtc: DateTime.UtcNow.AddMinutes(ExpirationTimeInMinutes),
                  status: InviteStatus.Open)
        {
        }

        public Invite(EntityId id, EntityId tenantId, Email to, Role role, Guid token, DateTime createdAtUtc, DateTime sentAtUtc, DateTime expiresAtUtc, InviteStatus status, DateTime? updatedAtUtc = null)
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

        protected override void ConfigureValidationRules(IValidationRuleFactory<Invite> validator)
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
            RaiseDomainEvent(new InviteRenewed(Token, To, ExpiresAtUtc));
        }

        public void Accept()
        {
            if (Status != InviteStatus.Open)
                throw new DomainException("This invitation has already been accepted or is inactive.");

            if (DateTime.UtcNow > ExpiresAtUtc)
                throw new DomainException("This invitation has expired.");

            UpdatedAtUtc = DateTime.UtcNow;
            Status = InviteStatus.Accepted;
        }

        public static Invite New(int tenantId, string to, Role role)
        {
            var invite = new Invite(EntityId.AutoIncrement(), new EntityId(tenantId), new Email(to), role);
            invite.RaiseDomainEvent(new InviteCreated(invite.Token, invite.To, invite.ExpiresAtUtc));
            return invite;
        }

        public static Invite Load(
            int id,
            int tenantId,
            string to,
            Role role,
            Guid token,
            DateTime createdAtUtc,
            DateTime sentAtUtc,
            DateTime expiresAtUtc,
            InviteStatus status,
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
