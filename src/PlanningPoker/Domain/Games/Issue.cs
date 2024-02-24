#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Games;

public sealed class Issue : TenantableAggregateRoot
{
    private readonly List<UserGrade> _grades;

    private Issue(string id, string tenantId, string gameId, string name, string? description = null,
        string? link = null,
        List<UserGrade>? grades = null) : base(id, tenantId)
    {
        SetGame(gameId);
        SetName(name);
        Link = link;
        Description = description;
        _grades = grades ?? [];
    }

    public EntityId GameId { get; private set; } = EntityId.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Link { get; private set; }
    public IReadOnlyCollection<UserGrade> UserGrades => _grades.AsReadOnly();

    public void RegisterGrade(string userId, string grade)
    {
        if (!userId.IsPresent())
        {
            AddError(IssueErrors.InvalidUserId);
            return;
        }

        _grades.RemoveAll(g => g.UserId.Value == userId);
        _grades.Add(new UserGrade(userId, grade));
    }

    public void SetName(string name)
    {
        if (!name.HasMinLength(3))
        {
            AddError(IssueErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public void SetGame(string gameId)
    {
        if (GameId.Value.IsPresent())
        {
            AddError(IssueErrors.ChangeIssueGame);
            return;
        }

        if (!gameId.IsPresent())
        {
            AddError(IssueErrors.InvalidGameId);
            return;
        }

        GameId = gameId;
    }

    public static Issue New(string tenantId, string gameId, string name, string? description = null,
        string? link = null)
    {
        return new Issue(EntityId.Generate(), tenantId, gameId, name, description, link);
    }
}