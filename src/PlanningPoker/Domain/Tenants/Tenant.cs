#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Tenants;

public class Tenant : AggregateRoot
{
    private Tenant(string id, string name)
        : base(id)
    {
        SetName(name);
    }

    public string Name { get; private set; } = string.Empty;

    public void SetName(string name)
    {
        if (!name.HasMinLength(3))
        {
            AddError(TenantErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public static Tenant Load(string id, string name)
    {
        return new Tenant(id, name);
    }

    public static Tenant New(string name)
    {
        return new Tenant(EntityId.Generate(), name);
    }
}