using PlanningPoker.Application.Abstractions.Commands;

namespace PlanningPoker.Application.Games.CreateIssue
{
    public class CreateIssueCommand(int gameId, string name, string? link = null, string? description = null)
        : Command
    {
        public int GameId { get; } = gameId;
        public string Name { get; } = name;
        public string? Link { get; } = link;
        public string? Description { get; } = description;
    }
}