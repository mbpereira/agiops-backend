#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Plugins;

public static class PluginErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(Plugin),
        nameof(Plugin.Name), 3);
}