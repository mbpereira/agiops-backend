using PlanningPoker.Application.Abstractions;

namespace PlanningPoker.Application.Users.CreateTenant
{
    public class CreateTenantCommand : Command
    {
        public string Name { get; private set; }

        public CreateTenantCommand(string name)
        {
            Name = name;
        }
    }
}
