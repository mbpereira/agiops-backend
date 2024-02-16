using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class AccessGrantErrors
    {
        public static Error InvalidUserId = Error.GreaterThan(nameof(AccessGrant), nameof(AccessGrant.UserId), value: 0);
        public static Error UserChange = new(nameof(AccessGrant), nameof(AccessGrant.UserId), "You cannot change user id");
    }
}
