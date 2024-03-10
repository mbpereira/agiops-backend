#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Plugins;

public sealed class Plugin : TenantableAggregateRoot
{
    private Plugin(string id, string tenantId, string name, PluginType type, Credentials credentials) : base(id,
        tenantId)
    {
        SetName(name);
        Type = type;
        Credentials = credentials;
    }

    public string Name { get; private set; } = string.Empty;
    public Credentials? Credentials { get; private set; }
    public PluginType Type { get; set; }

    public void SetName(string name)
    {
        if (!name.HasMinLength(3))
        {
            AddError(PluginErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public static Plugin NewWithApiToken(string tenantId, string name, PluginType type, string apiToken)
    {
        return new Plugin(EntityId.Generate(), tenantId, name, type, new ApiTokenCredentials(apiToken));
    }
}