#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Application.Games.AddGame;

public static class AddGameCommandErrors
{
    public static readonly Error InvalidVotingSystemId = Error.GreaterThan(nameof(AddGameCommand),
        nameof(AddGameCommand.VotingSystemId));
}