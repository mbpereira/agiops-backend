using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class TenantScopes
    {
        public static GrantScope[] Admin => new[] { GrantScope.Delete, GrantScope.View, GrantScope.Archive, GrantScope.Edit };
        public static GrantScope[] Viewer => new[] { GrantScope.View };
    }

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
