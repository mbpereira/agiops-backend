using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public static class VotingSystemErrors
    {
        public static readonly Error InvalidSharingStatus = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Undefined is not a valid status.");
        public static readonly Error InvalidSharingApprovalOperation = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Only the statuses 'requested' and 'rejected' can be approved.");
        public static readonly Error InvalidSharingRejectOperation = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Only the status 'requested' can be rejected.");
        public static readonly Error InvalidSharingRequestOperation = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Only the statuses 'rejected' and 'unshared' can made a request sharing.");
        public static readonly Error InvalidOwnerChangeOperation = new(nameof(VotingSystem), nameof(VotingSystem.UserId), "You cannot change the voting system owner, as it has already been set.");
        public static readonly Error InvalidOwnerId = Error.GreaterThan(nameof(VotingSystem), nameof(VotingSystem.UserId), value: 0);
        public static readonly Error InvalidGrades = Error.EmptyCollection(nameof(VotingSystem), "Grades");
        public static readonly Error InvalidDescription = Error.MinLength(nameof(VotingSystem), nameof(VotingSystem.Description), minLength: 3);
    }
}
