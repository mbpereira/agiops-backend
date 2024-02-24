#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Users;

public static class UserErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(User), nameof(User.Name), 3);

    public static readonly Error InvalidEmail = Error.InvalidEmail(nameof(User), nameof(User.Email));
}