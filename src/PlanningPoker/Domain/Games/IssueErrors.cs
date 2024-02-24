#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Games;

public static class IssueErrors
{
    public static readonly Error InvalidUserId =
        Error.NullOrEmpty(nameof(Issue), "UserId");

    public static readonly Error InvalidName = Error.MinLength(nameof(Issue), nameof(Issue.Name), 3);

    public static readonly Error ChangeIssueGame = new(nameof(Issue), nameof(Issue.GameId),
        "You cannot change the issue game, as it has already been set.");

    public static readonly Error InvalidGameId = Error.NullOrEmpty(nameof(Issue), nameof(Issue.GameId));
}