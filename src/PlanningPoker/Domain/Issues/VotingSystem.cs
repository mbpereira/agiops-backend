using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Issues
{
    public class VotingSystem : AggregateRoot<VotingSystem>
    {
        private readonly IList<int> _grades = new List<int>();
        public IReadOnlyCollection<int> Grades => _grades.AsReadOnly();
        public string Description { get; private set; }
        public int UserId { get; private set; }
        public bool Shared { get; private set; }
        public bool Revised { get; private set; }

        private VotingSystem(EntityId id, string description, int userId, bool shared, bool revised) : base(id)
        {
            Description = description;
            UserId = userId;
            Shared = shared;
            Revised = revised;
        }

        private VotingSystem(string description, int userId, bool shared)
            : this(EntityId.AutoIncrement(), description, userId, shared, revised: false)
        {
        }

        protected override void ConfigureValidationRules(Validator<VotingSystem> validator)
        {
            validator.RuleFor(v => v.Grades)
                .NotEmpty();
            validator.RuleFor(v => v.Description)
                .NotEmpty()
                .MinimumLength(3);
        }

        public static VotingSystem Load(EntityId id, string description, int userId, bool shared, bool revised) => new(id, description, userId, shared, revised);
        public static VotingSystem New(string description, int userId, bool shared) => new(description, userId, shared);

        public void AddGrade(int grade) => _grades.Add(grade);
        public void ClearGrades() => _grades.Clear();
        public void ChangeDescription(string description) => Description = description;
        public void Share(bool share) => Shared = share;
        public void Revise(bool revised) => Revised = revised;
    }
}
