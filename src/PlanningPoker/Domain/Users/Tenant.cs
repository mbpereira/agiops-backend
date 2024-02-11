using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public class Tenant : AggregateRoot<Tenant>
    {
        public string Name { get; private set; }

        private Tenant(EntityId id, string name)
            : base(id)
        {
            Name = name;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<Tenant> validator)
        {
            validator.CreateRuleFor(t => t.Name)
                .NotEmpty()
                .MinimumLength(3);
        }

        public static Tenant New(int id, string name) => new(id, name);
        public static Tenant New(string name) => new(EntityId.AutoIncrement(), name);
    }
}
