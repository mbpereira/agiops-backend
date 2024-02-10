using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class Game : AggregateRoot<Game>, ITenantable
    {
        public string Name { get; private set; }
        public EntityId UserId { get; private set; }
        public GameCredentials? Credentials { get; private set; }
        public EntityId TenantId { get; private set; }

        public Game(EntityId id, EntityId tenantId, string name, EntityId userId, string? password = null)
            : base(id)
        {
            Name = name;
            UserId = userId;
            TenantId = tenantId;
            DefinePassword(password);
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<Game> validator)
        {
            validator.CreateRuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(1);

            validator.CreateRuleFor(c => c.Credentials!.Password)
                .NotEmpty()
                .MinimumLength(6)
                .When(g => g.Credentials is not null);
        }

        public void DefinePassword(string? password = null)
        {
            Credentials = string.IsNullOrEmpty(password)
                ? null
                : new GameCredentials(password);
        }

        public static Game New(int tenantId, string name, int userId, string? password = null) 
            => new(EntityId.AutoIncrement(), new EntityId(tenantId), name, new EntityId(userId), password);
        public static Game New(int id, int tenantId, string name, int userId, string? password = null) 
            => new(new EntityId(id), new EntityId(tenantId), name, new EntityId(userId), password);
    }
}
