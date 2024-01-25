using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Issues
{
    public class Game : AggregateRoot<Game>
    {
        public string Name { get; private set; }
        /// <summary>
        /// game owner
        /// </summary>
        public int UserId { get; private set; }
        public GameCredentials? Credentials { get; private set; }

        public Game(string name, int userId, string? password = null)
            : this(id: EntityId.AutoIncrement(), name, userId, password)
        {
        }

        public Game(EntityId id, string name, int userId, string? password = null)
            : base(id)
        {
            Name = name;
            UserId = userId;
            DefinePassword(password);
        }

        protected override void ConfigureValidationRules(Validator<Game> validator)
        {
            validator.RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(1);

            validator.RuleFor(c => c.Credentials!.Password)
                .NotEmpty()
                .MinimumLength(6)
                .When(g => g.Credentials is not null);
        }

        public void DefinePassword(string? password = null)
        {
            Credentials = string.IsNullOrEmpty(password)
                ? null
                : new GameCredentials(password);
        }

        public static Game New(string name, int userId, string? password = null) => new(name, userId, password);

        public static Game Load(EntityId id, string name, int userId, string password) => new(id, name, userId, password);
    }
}
