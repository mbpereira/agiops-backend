using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Games
{
    public static class IssueErrors
    {
        public static readonly Error InvalidUserId =
            new(nameof(Issue), "UserId", "Provided user id is not valid.");

        public static readonly Error InvalidName = Error.MinLength(nameof(Issue), nameof(Issue.Name), minLength: 3);

        public static readonly Error ChangeIssueGame = new(nameof(Issue), nameof(Issue.GameId),
            "You cannot change the issue game, as it has already been set.");

        public static readonly Error InvalidGameId = Error.GreaterThan(nameof(Issue), nameof(Issue.GameId), value: 0);
    }
}