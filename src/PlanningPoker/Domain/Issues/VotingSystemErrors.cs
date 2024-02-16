using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public static class VotingSystemErrors
    {
        public static Error InvalidSharingStatus = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Undefined is not a valid status.");
        public static Error InvalidSharingApprovalOperation = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Only the statuses 'requested' and 'rejected' can be approved.");
        public static Error InvalidSharingRejectOperation = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Only the status 'requested' can be rejected.");
        public static Error InvalidSharingRequestOperation = new(nameof(VotingSystem), nameof(VotingSystem.SharingStatus), "Only the statuses 'rejected' and 'unshared' can made a request sharing.");
        public static Error InvalidOwnerChangeOperation = new(nameof(VotingSystem), nameof(VotingSystem.UserId), "You cannot change the voting system owner, as it has already been set.");
        public static Error InvalidOwnerId = Error.GreaterThan(nameof(VotingSystem), nameof(VotingSystem.UserId), value: 0);
        public static Error InvalidGrades = Error.EmptyCollection(nameof(VotingSystem), "Grades");
        public static Error InvalidDescription = Error.MinLength(nameof(VotingSystem), nameof(VotingSystem.Description), minLength: 3);
    }
}
