using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;

namespace PlanningPoker.Domain.Issues
{
    public sealed class VotingSystem : TenantableAggregateRoot
    {
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public string Description { get; private set; } = string.Empty;
        public GradeDetails GradeDetails { get; private set; } = GradeDetails.Empty();
        public SharingStatus SharingStatus { get; private set; } = SharingStatus.Undefined;

        private VotingSystem(int id, int tenantId, string description, int userId, IList<string> grades, SharingStatus sharingStatus) : base(id, tenantId)
        {
            Describe(description);
            SetOwner(userId);
            SetPossibleGrades(grades);
            SetSharingStatus(sharingStatus);
        }

        public void SetSharingStatus(SharingStatus newSharingStatus)
        {
            if (SharingStatus.Undefined.Equals(newSharingStatus))
            {
                AddError(VotingSystemErrors.InvalidSharingStatus);
                return;
            }

            if (SharingStatus.Approved.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Requested, SharingStatus.Rejected, SharingStatus.Undefined))
            {
                AddError(VotingSystemErrors.InvalidSharingApprovalOperation);
                return;
            }

            if (SharingStatus.Rejected.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Requested, SharingStatus.Undefined))
            {
                AddError(VotingSystemErrors.InvalidSharingRejectOperation);
                return;
            }

            if (SharingStatus.Requested.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Rejected, SharingStatus.Unshared, SharingStatus.Undefined))
            {
                AddError(VotingSystemErrors.InvalidSharingRequestOperation);
                return;
            }

            SharingStatus = newSharingStatus;
        }


        public void SetOwner(int userId)
        {
            if (UserId.Value.GreaterThan(0))
            {
                AddError(VotingSystemErrors.InvalidOwnerChangeOperation);
                return;
            }

            if (!userId.GreaterThan(0))
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

        public void Describe(string description)
        {
            if (!description.HasMinLength(minLength: 3))
            {
                AddError(VotingSystemErrors.InvalidDescription);
                return;
            }

            Description = description;
        }

        public static VotingSystem New(int tenantId, string description, int userId, IList<string> possibleGrades, SharingStatus sharing = SharingStatus.Unshared)
            => new(EntityId.AutoIncrement(), tenantId, description, userId, possibleGrades, sharing);

        public static VotingSystem Load(int id, int tenantId, string description, int userId, IList<string> possibleGrades, SharingStatus sharing = SharingStatus.Unshared)
            => new(id, tenantId, description, userId, possibleGrades, sharing);
    }
}
