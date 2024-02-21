using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;

namespace PlanningPoker.Domain.Issues
{
    public sealed class Issue : TenantableAggregateRoot
    {
        public EntityId GameId { get; private set; } = EntityId.Blank();
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Link { get; private set; }
        private readonly List<UserGrade> _grades;
        public IReadOnlyCollection<UserGrade> UserGrades => _grades.AsReadOnly();

        private Issue(int id, int tenantId, int gameId, string name, string? description = null, string? link = null,
            List<UserGrade>? grades = null) : base(id, tenantId)
        {
            SetGame(gameId);
            SetName(name);
            Link = link;
            Description = description;
            _grades = grades ?? new List<UserGrade>();
        }

        public void RegisterGrade(int userId, string grade)
        {
            if (!userId.GreaterThan(0))
            {
                AddError(IssueErrors.InvalidUserId);
                return;
            }

            _grades.RemoveAll(g => g.UserId.Value == userId);
            _grades.Add(new UserGrade(new EntityId(userId), grade));
        }

        public void SetName(string name)
        {
            if (!name.HasMinLength(minLength: 3))
            {
                AddError(IssueErrors.InvalidName);
                return;
            }

            Name = name;
        }

        public void SetGame(int gameId)
        {
            if (GameId.Value.GreaterThan(0))
            {
                AddError(IssueErrors.ChangeIssueGame);
                return;
            }

            if (!gameId.GreaterThan(0))
            {
                AddError(IssueErrors.InvalidGameId);
                return;
            }

            GameId = gameId;
        }

        public static Issue New(int tenantId, int gameId, string name, string? description = null, string? link = null)
            => new(EntityId.AutoIncrement(), tenantId, gameId, name, description, link);

        public static Issue Load(int id, int tenantId, int gameId, string name, string? description = null,
            string? link = null, List<UserGrade>? grades = null)
            => new(id, tenantId, gameId, name, description, link, grades);
    }
}