using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

namespace PlanningPoker.Domain.Games
{
    public sealed class VotingSystem : TenantableAggregateRoot
    {
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public string Name { get; private set; } = string.Empty;
        public GradeDetails GradeDetails { get; private set; } = GradeDetails.Empty();
        public SharingStatus SharingStatus { get; private set; } = SharingStatus.Undefined;

        private VotingSystem(int id, int tenantId, string name, int userId, IList<string> grades,
            SharingStatus sharingStatus) : base(id, tenantId)
        {
            SetName(name);
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

        public void SetName(string name)
        {
            if (!name.HasMinLength(minLength: 3))
            {
                AddError(VotingSystemErrors.InvalidName);
                return;
            }

            Name = name;
        }

        public static VotingSystem New(int tenantId, string description, int userId, IList<string> possibleGrades,
            SharingStatus sharing = SharingStatus.Unshared)
            => new(EntityId.AutoIncrement(), tenantId, description, userId, possibleGrades, sharing);

        public static VotingSystem Load(int id, int tenantId, string description, int userId,
            IList<string> possibleGrades, SharingStatus sharing = SharingStatus.Unshared)
            => new(id, tenantId, description, userId, possibleGrades, sharing);
    }
}