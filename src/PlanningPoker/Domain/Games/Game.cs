using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Games
{
    public class Game : Entity<Game>
    {
        public string Name { get; private set; }

        public Game(string name)
            : this(id: EntityId.AutoIncrement(), name)
        {
        }

        public Game(EntityId id, string name) 
            : base(id)
        {
            Id = id;
            Name = name;
        }

        protected override void ConfigureValidationRules(Validator<Game> validator)
        {
            validator.RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(1);
        }

        public static Game New(string name) => new(name);

        public static Game Load(EntityId id, string name) => new(id, name);
    }
}
