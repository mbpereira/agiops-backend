namespace PlanningPoker.Application.Issues.CreateGame
{
    public record CreateGameCommand(string Name, int UserId, string? Password = null);
}