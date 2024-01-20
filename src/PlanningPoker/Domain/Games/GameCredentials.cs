using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Games
{
    public class GameCredentials : Entity<GameCredentials>
    {
        public int GameId { get; private set; }
        public string Password { get; private set; }

        private GameCredentials(int gameId, string password)
            : this(id: Constants.AutoIncrement, gameId, password)
        {
        }

        private GameCredentials(int id, int gameId, string password)
        {
            Id = id;
            GameId = gameId;
            Password = password;
        }

        protected override void ConfigureValidationRules(Validator<GameCredentials> validator)
        {
            validator.RuleFor(g => g.GameId)
                .GreaterThan(0);
            validator.RuleFor(g => g.Password)
                .NotEmpty()
                .MinimumLength(6);
        }

        public static GameCredentials New(int gameId, string password) => new(gameId, password);

        public static GameCredentials Load(int id, int gameId, string password) => new(id, gameId, password);
    }
}
