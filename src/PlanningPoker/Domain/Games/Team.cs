#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Games;

public class Team : TenantableAggregateRoot
{
    private Team(string id, string tenantId, string name) : base(id, tenantId)
    {
        SetName(name);
    }

    public string Name { get; private set; } = string.Empty;

    private void SetName(string name)
    {
        if (!name.HasMinLength(3))
        {
            AddError(TeamErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public static Team New(string tenantId, string name)
    {
        return new Team(EntityId.Generate(), tenantId, name);
    }
}