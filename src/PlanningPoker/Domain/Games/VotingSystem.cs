#region

using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Games;

public sealed class VotingSystem : TenantableAggregateRoot
{
    private VotingSystem(string id, string tenantId, string name, string userId, IList<string> grades,
        SharingStatus sharingStatus) : base(id, tenantId)
    {
        SetName(name);
        SetOwner(userId);
        SetPossibleGrades(grades);
        SetSharingStatus(sharingStatus);
    }

    public EntityId UserId { get; private set; } = EntityId.Empty;
    public string Name { get; private set; } = string.Empty;
    public GradeDetails GradeDetails { get; private set; } = GradeDetails.Empty();
    public SharingStatus SharingStatus { get; private set; } = SharingStatus.Undefined;

    public void SetSharingStatus(SharingStatus newSharingStatus)
    {
        if (SharingStatus.Undefined.Equals(newSharingStatus))
        {
            AddError(VotingSystemErrors.InvalidSharingStatus);
            return;
        }

        if (SharingStatus.Approved.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Requested,
                SharingStatus.Rejected, SharingStatus.Undefined))
        {
            AddError(VotingSystemErrors.InvalidSharingApprovalOperation);
            return;
        }

        if (SharingStatus.Rejected.Equals(newSharingStatus) &&
            !SharingStatus.IsSome(SharingStatus.Requested, SharingStatus.Undefined))
        {
            AddError(VotingSystemErrors.InvalidSharingRejectOperation);
            return;
        }

        if (SharingStatus.Requested.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Rejected,
                SharingStatus.Unshared, SharingStatus.Undefined))
        {
            AddError(VotingSystemErrors.InvalidSharingRequestOperation);
            return;
        }

        SharingStatus = newSharingStatus;
    }


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

    public static VotingSystem New(string tenantId, string description, string userId, IList<string> possibleGrades,
        SharingStatus sharing = SharingStatus.Unshared)
    {
        return new VotingSystem(EntityId.Generate(), tenantId, description, userId, possibleGrades, sharing);
    }
}