using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.CreateTenant
{
    public class CreateTenantCommand : Command<CreateTenantCommand>
    {
        public string Name { get; private set; }

        public CreateTenantCommand(string name)
        {
            Name = name;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<CreateTenantCommand> validator)
        {
        }
    }
}
