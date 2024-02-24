#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Users;

public static class AccessGrantErrors
{
    public static readonly Error InvalidUserId =
        Error.GreaterThan(nameof(AccessGrant), nameof(AccessGrant.UserId));

    public static readonly Error UserChange = new(nameof(AccessGrant), nameof(AccessGrant.UserId),
        "You cannot change user id");
}