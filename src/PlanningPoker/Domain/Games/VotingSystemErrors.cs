#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Games;

public static class VotingSystemErrors
{
    public static readonly Error InvalidOwnerChangeOperation = new(nameof(VotingSystem),
        nameof(VotingSystem.UserId), "You cannot change the voting system owner, as it has already been set.");

    public static readonly Error InvalidOwnerId =
        Error.NullOrEmpty(nameof(VotingSystem), nameof(VotingSystem.UserId));

    public static readonly Error InvalidGrades = Error.EmptyCollection(nameof(VotingSystem), "Grades");

    public static readonly Error InvalidName =
        Error.MinLength(nameof(VotingSystem), nameof(VotingSystem.Name), 3);
}