using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class VotingSystem : AggregateRoot<VotingSystem>
    {
        private readonly IList<int> _grades = new List<int>();
        public IReadOnlyCollection<int> Grades => _grades.AsReadOnly();
        public string Description { get; private set; }
        public EntityId UserId { get; private set; }
        public bool Shared { get; private set; }
        public bool Revised { get; private set; }

        private VotingSystem(EntityId id, string description, EntityId userId, bool shared, bool revised) : base(id)
        {
            Description = description;
            UserId = userId;
            Shared = shared;
            Revised = revised;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<VotingSystem> validator)
        {
            validator.CreateFor(v => v.Grades)
                .NotEmpty();

            validator.CreateFor(v => v.Description)
                .NotEmpty()
                .MinimumLength(3);

            validator.CreateFor(v => v.UserId!.Value, propertyName: nameof(UserId))
                .GreaterThan(0);
        }

        public static VotingSystem New(string description, int userId, bool shared) => new(EntityId.AutoIncrement(), description, new EntityId(userId), shared, revised: false);

        public void AddGrade(int grade) => _grades.Add(grade);
        public void ClearGrades() => _grades.Clear();
        public void ChangeDescription(string description) => Description = description;
        public void Share(bool shared) => Shared = shared;
        public void Revise(bool revised) => Revised = revised;
    }
}
