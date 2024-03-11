#region

using PlanningPoker.Application.Abstractions.Commands;

#endregion

namespace PlanningPoker.Application.Tenants.AddTenant;

public class AddTenantCommand(string name) : Command
{
    public string Name { get; private set; } = name;
}