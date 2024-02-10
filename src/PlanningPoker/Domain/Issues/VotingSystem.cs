using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class VotingSystem : AggregateRoot<VotingSystem>, ITenantable
    {
        private readonly IList<int> _grades = new List<int>();
        public IReadOnlyCollection<int> Grades => _grades.AsReadOnly();
        public string Description { get; private set; }
        public EntityId UserId { get; private set; }
        public EntityId TenantId { get; private set; }

        private VotingSystem(EntityId id, EntityId tenantId, string description, EntityId userId) : base(id)
        {
            Description = description;
            UserId = userId;
            TenantId = tenantId;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<VotingSystem> validator)
        {
            validator.CreateRuleFor(v => v.Grades)
                .NotEmpty();

            validator.CreateRuleFor(v => v.Description)
                .NotEmpty()
                .MinimumLength(3);

            validator.CreateRuleFor(v => v.UserId!.Value, propertyName: nameof(UserId))
                .GreaterThan(0);
        }

        public static VotingSystem New(int tenantId, string description, int userId)
            => new(EntityId.AutoIncrement(), new EntityId(tenantId), description, new EntityId(userId));

        public void AddGrade(int grade) => _grades.Add(grade);
        public void ClearGrades() => _grades.Clear();
        public void ChangeDescription(string description) => Description = description;
    }
}
