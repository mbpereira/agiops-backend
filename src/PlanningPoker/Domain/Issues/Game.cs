using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Issues
{
    public sealed class Game : AggregateRoot<Game>
    {
        public string Name { get; private set; }
        /// <summary>
        /// game owner
        /// </summary>
        public EntityId UserId { get; private set; }
        public GameCredentials? Credentials { get; private set; }

        private Game(EntityId id, string name, EntityId userId, string? password = null)
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

        public static Game New(string name, int userId, string? password = null) => new(EntityId.AutoIncrement(), name, new EntityId(userId), password);

        public static Game Load(int id, string name, int userId, string password) => new(new EntityId(id), name, new EntityId(userId), password);
    }
}
