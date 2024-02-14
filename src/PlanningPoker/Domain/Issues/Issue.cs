using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class Issue : TenantableAggregateRoot
    {
        public EntityId GameId { get; private set; } = EntityId.Blank();
        public string Name { get; private set; } = string.Empty;
        public string? Link { get; private set; }
        public string? Description { get; private set; }
        private readonly List<UserGrade> _grades;
        public IReadOnlyCollection<UserGrade> Grades => _grades.AsReadOnly();
        public decimal Average => Grades.Average(g => g.Grade);

        private Issue(int id, int tenantId, int gameId, string name, string? description = null, string? link = null) : base(id, tenantId)
        {
            DefineGame(gameId);
            DefineName(name);
            Link = link;
            Description = description;
            _grades = new List<UserGrade>(capacity: 9);
        }

        public void RegisterGrade(int userId, decimal grade)
        {
            if (!userId.GreaterThan(0))
            {
                AddError(new Error(nameof(Issue), nameof(RegisterGrade), "Provided user id is not valid."));
                return;
            }

            _grades.RemoveAll(g => g.UserId.Value == userId);
            _grades.Add(new UserGrade(new EntityId(userId), grade));
        }

        public void DefineName(string name)
        {
            if (!name.HasMinLength(minLength: 3))
            {
                AddError(Error.MinLength(nameof(Issue), nameof(name), minLength: 3));
                return;
            }

            Name = name;
        }

        public void DefineGame(int gameId)
        {
            if (GameId.Value.GreaterThan(0)) return;

            if (!gameId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(Issue), nameof(gameId), value: 0));
                return;
            }

            GameId = gameId;
        }

        public static Issue New(int tenantId, int gameId, string name, string? description = null, string? link = null)
            => new(EntityId.AutoIncrement(), tenantId, gameId, name, description, link);
        public static Issue New(int id, int tenantId, int gameId, string name, string? description = null, string? link = null)
            => new(id, tenantId, gameId, name, description, link);
    }
}
