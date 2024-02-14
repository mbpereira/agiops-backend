using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.CreateGame
{
    public class CreateGameCommand : Command
    {
        public string Name { get; }
        public string? Password { get; }

        public CreateGameCommand(string name, string? password = null)
        {
            Name = name;
            Password = password;
        }
    }
}