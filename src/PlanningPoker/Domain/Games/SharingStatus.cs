namespace PlanningPoker.Domain.Games;

public enum SharingStatus
{
    Undefined = -1,
    Unshared = 1,
    Requested = 2,
    Approved = 3,
    Rejected = 4
}