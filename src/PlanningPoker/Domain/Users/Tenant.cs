using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public class Tenant : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;

        private Tenant(int id, string name)
            : base(id)
        {
            Named(name);
        }

        public void Named(string name)
        {
            if (!name.HasMinLength(minLength: 3))
            {
                AddError(Error.MinLength(nameof(Tenant), nameof(name), minLength: 3));
                return;
            }

            Name = name;
        }

        public static Tenant Load(int id, string name) => new(id, name);
        public static Tenant New(string name) => new(EntityId.AutoIncrement(), name);
    }
}
