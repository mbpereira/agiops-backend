using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Issues
{
    public sealed class Issue : AggregateRoot<Issue>
    {
        public int GameId { get; private set; }
        public string Name { get; private set; }
        public string? Link { get; private set; }
        public string? Description { get; private set; }

        private readonly List<UserGrade> _grades;
        public IReadOnlyCollection<UserGrade> Grades => _grades.AsReadOnly();

        public decimal Average => Grades.Average(g => g.Grade);

        private Issue(EntityId id, int gameId, string name, string? description = null, string? link = null) : base(id)
        {
            GameId = gameId;
            Name = name;
            Link = link;
            Description = description;
            _grades = new List<UserGrade>(capacity: 9);
        }

        public static Issue New(int gameId, string name, string? description = null, string? link = null) => new(EntityId.AutoIncrement(), gameId, name, description, link);

        public static Issue Load(int id, int gameId, string name, string? description = null, string? link = null) => new(new EntityId(id), gameId, name, description, link);

        public void RegisterGrade(int userId, decimal grade)
        {
            _grades.RemoveAll(g => g.UerId.Value == userId);
            _grades.Add(new UserGrade(new EntityId(userId), grade));
        }

        protected override void ConfigureValidationRules(Validator<Issue> validator)
        {
            validator.RuleFor(i => i.GameId)
                .GreaterThan(0);

            validator.RuleFor(i => i.Name)
                .NotEmpty()
                .MinimumLength(3);
        }
    }
}
