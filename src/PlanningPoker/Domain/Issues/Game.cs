using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Issues
{
    public class Game : AggregateRoot<Game>
    {
        public string Name { get; private set; }
        /// <summary>
        /// same as userId
        /// </summary>
        public int OwnerId { get; private set; }
        public GameCredentials? Credentials { get; private set; }

        public Game(string name, int ownerId, string? password = null)
            : this(id: EntityId.AutoIncrement(), name, ownerId, password)
        {
        }

        public Game(EntityId id, string name, int ownerId, string? password = null)
            : base(id)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            DefinePassword(password);
        }

        protected override void ConfigureValidationRules(Validator<Game> validator)
        {
            validator.RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(1);

            if (Credentials is not null)
            {
                validator.RuleFor(c => c.Credentials!.Password)
                    .NotEmpty()
                    .MinimumLength(6);
            }
        }

        public void DefinePassword(string? password = null)
        {
            Credentials = string.IsNullOrEmpty(password)
                ? null
                : new GameCredentials(password);
        }

        public static Game New(string name, int ownerId, string? password = null) => new(name, ownerId, password);

        public static Game Load(EntityId id, string name, int ownerId, string password) => new(id, name, ownerId, password);
    }
}
