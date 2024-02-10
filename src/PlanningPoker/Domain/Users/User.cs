using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public sealed class User : AggregateRoot<User>, ITenantable
    {
        public string Name { get; private set; }
        public Email? Email { get; private set; }
        public Guest? Guest { get; private set; }
        public EntityId TenantId { get; private set; }

        public bool IsGuest => Guest is not null;


        private User(EntityId id, EntityId tenantId, string name, string? email, string? sessionId) : base(id)
        {
            Name = name;
            TenantId = tenantId;
            IdentifyUser(email, sessionId);
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<User> validator)
        {
            validator.CreateRuleFor(u => u.Name)
                .NotEmpty()
                .MinimumLength(3);
        }

        private void IdentifyUser(string? email, string? sessionId)
        {
            if (email is null && sessionId is null) throw new DomainException("Email or Session Id must be defined.");

            if (email is not null)
            {
                Email = new Email(email);
                Guest = null;
                return;
            }

            Guest = new Guest(sessionId!);
        }

        public static User New(int id, int tenantId, string name, string email) => new(new EntityId(id), new EntityId(tenantId), name, email, sessionId: null);
        public static User New(int tenantId, string name, string email) => new(EntityId.AutoIncrement(), new EntityId(tenantId), name, email, sessionId: null);
        public static User NewGuest(int tenantId, string name) => new(EntityId.AutoIncrement(), new EntityId(tenantId), name, email: null, sessionId: Guid.NewGuid().ToString());
    }
}
