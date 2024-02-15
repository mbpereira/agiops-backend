using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.CreateGame
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
                AddError(Error.GreaterThan(nameof(CreateGameCommand), nameof(votingSystemId), value: 0));
                return;
            }

            VotingSystemId = votingSystemId;
        }
    }
}