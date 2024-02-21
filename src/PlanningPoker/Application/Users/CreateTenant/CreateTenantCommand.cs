using PlanningPoker.Application.Abstractions;

namespace PlanningPoker.Application.Users.CreateTenant
{
    public class CreateTenantCommand(string name) : Command
    {
        public string Name { get; private set; } = name;
    }
}