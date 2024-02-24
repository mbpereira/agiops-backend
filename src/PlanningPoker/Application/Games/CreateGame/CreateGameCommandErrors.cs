#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Application.Games.CreateGame;

public static class CreateGameCommandErrors
{
    public static readonly Error InvalidVotingSystemId = Error.GreaterThan(nameof(CreateGameCommand),
        nameof(CreateGameCommand.VotingSystemId));
}