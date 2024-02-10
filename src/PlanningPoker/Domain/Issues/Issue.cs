using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class Issue : AggregateRoot<Issue>, ITenantable
    {
        public int GameId { get; private set; }
        public string Name { get; private set; }
        public string? Link { get; private set; }
        public string? Description { get; private set; }
        public EntityId TenantId { get; private set; }

        private readonly List<UserGrade> _grades;
        public IReadOnlyCollection<UserGrade> Grades => _grades.AsReadOnly();

        public decimal Average => Grades.Average(g => g.Grade);


        private Issue(EntityId id, EntityId tenantId, int gameId, string name, string? description = null, string? link = null) : base(id)
        {
            GameId = gameId;
            Name = name;
            Link = link;
            Description = description;
            _grades = new List<UserGrade>(capacity: 9);
            TenantId = tenantId;
        }

        public static Issue New(int tenantId, int gameId, string name, string? description = null, string? link = null)
            => new(EntityId.AutoIncrement(), new EntityId(tenantId), gameId, name, description, link);
        public static Issue New(int id, int tenantId, int gameId, string name, string? description = null, string? link = null)
            => new(new EntityId(id), new EntityId(tenantId), gameId, name, description, link);

        public void RegisterGrade(int userId, decimal grade)
        {
            if (userId <= 0) throw new DomainException("Provided user id is not valid.");

            _grades.RemoveAll(g => g.UerId.Value == userId);
            _grades.Add(new UserGrade(new EntityId(userId), grade));
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<Issue> validator)
        {
            validator.CreateRuleFor(i => i.GameId)
                .GreaterThan(0);

            validator.CreateRuleFor(i => i.Name)
                .NotEmpty()
                .MinimumLength(3);
        }
    }
}
