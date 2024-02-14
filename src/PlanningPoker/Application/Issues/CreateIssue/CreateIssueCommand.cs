using PlanningPoker.Application.Abstractions;

namespace PlanningPoker.Application.Issues.CreateIssue
{
    public class CreateIssueCommand : Command
    {
        public int GameId { get; }
        public string Name { get; }
        public string? Link { get; }
        public string? Description { get; }

        public CreateIssueCommand(int gameId, string name, string? link = null, string? description = null)
        {
            GameId = gameId;
            Name = name;
            Link = link;
            Description = description;
        }
    }
}