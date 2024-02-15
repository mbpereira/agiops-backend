using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public sealed class VotingSystem : TenantableAggregateRoot
    {
        public EntityId UserId { get; private set; } = EntityId.Blank();
        public string Description { get; private set; } = string.Empty;
        public IList<int> Grades { get; private set; } = new List<int>();
        public SharingStatus SharingStatus { get; private set; } = SharingStatus.Undefined;

        private VotingSystem(int id, int tenantId, string description, int userId, IList<int> grades, SharingStatus sharingStatus) : base(id, tenantId)
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
                AddError(new Error(nameof(VotingSystem), nameof(newSharingStatus), "Undefined is not a valid status."));
                return;
            }

            if (SharingStatus.Approved.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Requested, SharingStatus.Rejected, SharingStatus.Undefined))
            {
                AddError(new Error(nameof(VotingSystem), nameof(newSharingStatus), "Only the statuses 'requested' and 'rejected' can be approved."));
                return;
            }

            if (SharingStatus.Rejected.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Requested, SharingStatus.Undefined))
            {
                AddError(new Error(nameof(VotingSystem), nameof(newSharingStatus), "Only the status 'requested' can be rejected."));
                return;
            }

            if (SharingStatus.Requested.Equals(newSharingStatus) && !SharingStatus.IsSome(SharingStatus.Rejected, SharingStatus.Unshared, SharingStatus.Undefined))
            {
                AddError(new Error(nameof(VotingSystem), nameof(newSharingStatus), "Only the statuses 'cancelled', 'rejected' and 'unshared' can made a request sharing."));
                return;
            }

            SharingStatus = newSharingStatus;
        }


        public void SetOwner(int userId)
        {
            if (UserId.Value.GreaterThan(0))
            {
                AddError(new Error(nameof(VotingSystem), nameof(userId), VotingSystemConstants.Messages.OwnerAlreadySetd));
                return;
            }

            if (!userId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(VotingSystem), nameof(userId), value: 0));
                return;
            }

            UserId = new EntityId(userId);
        }

        public void SetPossibleGrades(IList<int> grades)
        {
            if (grades.IsEmpty())
            {
                AddError(Error.EmptyCollection(nameof(VotingSystem), nameof(grades)));
                return;
            }

            Grades = grades.AsReadOnly();
        }

        public void Describe(string description)
        {
            if (!description.HasMinLength(minLength: 3))
            {
                AddError(Error.MinLength(nameof(VotingSystem), nameof(description), minLength: 3));
                return;
            }

            Description = description;
        }

        public static VotingSystem New(int tenantId, string description, int userId, IList<int> grades, SharingStatus sharing = SharingStatus.Unshared)
            => new(EntityId.AutoIncrement(), tenantId, description, userId, grades, sharing);

        public static VotingSystem Load(int id, int tenantId, string description, int userId, IList<int> grades, SharingStatus sharing = SharingStatus.Unshared)
            => new(id, tenantId, description, userId, grades, sharing);
    }
}
