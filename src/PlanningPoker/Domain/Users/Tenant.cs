using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public class Tenant : AggregateRoot<Tenant>
    {
        public string Name { get; private set; }
        private readonly IList<TenantMaintainer> _maintainers;
        public IReadOnlyCollection<TenantMaintainer> Maintainers => _maintainers.AsReadOnly();


        private Tenant(EntityId id, string name)
            : base(id)
        {
            Name = name;
            _maintainers = new List<TenantMaintainer>();
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<Tenant> validator)
        {
            validator.CreateRuleFor(t => t.Name)
                .NotEmpty()
                .MinimumLength(3);
        }

        public void AddMaintainer(int userId)
        {
            if (userId <= 0) throw new DomainException("Provided user id is not valid");

            if (_maintainers.Any(m => m.UserId.Value == userId)) return;

            _maintainers.Add(new TenantMaintainer(new EntityId(userId)));
        }

        public static Tenant New(int id, string name) => new(id, name);
        public static Tenant New(string name) => new(EntityId.AutoIncrement(), name);
    }
}
