#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Games;

public static class TeamErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(Team), nameof(Team.Name), 3);
}