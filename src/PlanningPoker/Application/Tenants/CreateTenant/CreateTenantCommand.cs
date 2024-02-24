#region

using PlanningPoker.Application.Abstractions.Commands;

#endregion

namespace PlanningPoker.Application.Tenants.CreateTenant;

public class CreateTenantCommand(string name) : Command
{
    public string Name { get; private set; } = name;
}