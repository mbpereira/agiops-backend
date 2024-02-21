using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;

namespace PlanningPoker.Application.Games.CreateGame
{
    public class CreateGameCommand : Command
    {
        public string Name { get; private set; }
        public string? Password { get; private set; }
        public int VotingSystemId { get; private set; }

        public CreateGameCommand(string name, int votingSystemId, string? password = null)
        {
            Name = name;
            Password = password;
            SetVotingSystemId(votingSystemId);
        }

        private void SetVotingSystemId(int votingSystemId)
        {
            if (!votingSystemId.GreaterThan(0))
            {
                AddError(CreateGameCommandErrors.InvalidVotingSystemId);
                return;
            }

            VotingSystemId = votingSystemId;
        }
    }
}