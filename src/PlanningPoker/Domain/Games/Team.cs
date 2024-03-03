using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

namespace PlanningPoker.Domain.Games;

public class Team : TenantableAggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    private Team(string id, string tenantId, string name) : base(id, tenantId)
    {
        SetName(name);
    }

    private void SetName(string name)
    {
        if (!name.HasMinLength(minLength: 3))
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