using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.CreateGame
{
    public class CreateGameCommand : Command<CreateGameCommand>
    {
        public string Name { get; }
        public int UserId { get; }
        public string? Password { get; }

        public CreateGameCommand(string name, int userId, string? password = null)
        {
            Name = name;
            UserId = userId;
            Password = password;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<CreateGameCommand> validator)
        {
        }
    }
}