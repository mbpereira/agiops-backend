using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users.Events;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public class Invite : AggregateRoot<Invite>, ITenantable
    {
        public Email To { get; private set; }
        public Guid Token { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public EntityId TenantId { get; private set; }

        private Invite(EntityId id, EntityId tenantId, Email to)
            : base(id)
        {
            To = to;
            Token = Guid.NewGuid();
            TenantId = tenantId;
            CreatedAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = CreatedAtUtc.AddMinutes(30);
            RaiseDomainEvent(new InviteCreated(Token, To, ExpiresAtUtc));
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<Invite> validator)
        {
            validator.CreateRuleFor(i => i.To.Value, nameof(To))
                .NotEmpty()
                .NotNull()
                .EmailAddress();
        }

        public static Invite New(int tenantId, string to) => new(EntityId.AutoIncrement(), new EntityId(tenantId), new Email(to));
        public static Invite New(int id, int tenantId, string to) => new(new EntityId(id), new EntityId(tenantId), new Email(to));
    }
}
