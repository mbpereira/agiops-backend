using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.CreateGame;

public static class CreateGameCommandErrors
{
    public static readonly Error InvalidVotingSystemId = Error.GreaterThan(nameof(CreateGameCommand),
        nameof(CreateGameCommand.VotingSystemId), value: 0);
}