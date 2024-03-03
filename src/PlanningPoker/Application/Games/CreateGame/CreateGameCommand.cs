#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Application.Games.CreateGame;

public class CreateGameCommand : Command
{
    public CreateGameCommand(string name, string votingSystemId, string? password = null, string? teamId = null)
    {
        Name = name;
        Password = password;
        SetVotingSystemId(votingSystemId);
        TeamId = teamId;
    }

    public string Name { get; private set; }
    public string? Password { get; private set; }
    public string VotingSystemId { get; private set; } = string.Empty;
    public string? TeamId { get; set; }

    private void SetVotingSystemId(string votingSystemId)
    {
        if (!votingSystemId.IsPresent())
        {
            AddError(CreateGameCommandErrors.InvalidVotingSystemId);
            return;
        }

        VotingSystemId = votingSystemId;
    }
}