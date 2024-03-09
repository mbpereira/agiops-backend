using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Plugins;

public static class PluginErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(Plugin),
        nameof(Plugin.Name), minLength: 3);
}