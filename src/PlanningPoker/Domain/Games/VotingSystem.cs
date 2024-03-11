#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Games;

public sealed class VotingSystem : TenantableAggregateRoot
{
    private VotingSystem(string id, string tenantId, string name, string userId, IList<string> grades,
        string? description = null) : base(id,
        tenantId)
    {
        SetName(name);
        SetOwner(userId);
        SetPossibleGrades(grades);
        SetDescription(description);
    }

    public EntityId UserId { get; private set; } = EntityId.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public GradeDetails GradeDetails { get; private set; } = GradeDetails.Empty();

    public void SetOwner(string userId)
    {
        if (UserId.Value.IsPresent())
        {
            AddError(VotingSystemErrors.InvalidOwnerChangeOperation);
            return;
        }

        if (!userId.IsPresent())
        {
            AddError(VotingSystemErrors.InvalidOwnerId);
            return;
        }

        UserId = new EntityId(userId);
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetPossibleGrades(IList<string> grades)
    {
        var validGrades = grades.OnlyNotNullOrEmpty();

        if (validGrades.IsEmpty())
        {
            AddError(VotingSystemErrors.InvalidGrades);
            return;
        }

        GradeDetails = new GradeDetails(grades);
    }

    public void SetName(string name)
    {
        if (!name.HasMinLength(3))
        {
            AddError(VotingSystemErrors.InvalidName);
            return;
        }

        Name = name;
    }

    public static VotingSystem New(string tenantId, string name, string userId, IList<string> possibleGrades,
        string? description = null)
    {
        return new VotingSystem(EntityId.Generate(), tenantId, name, userId, possibleGrades, description);
    }
}