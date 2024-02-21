﻿using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error InvalidName = Error.MinLength(nameof(User), nameof(User.Name), minLength: 3);
        public static readonly Error InvalidIdentification = new(nameof(User), nameof(User.IdentifyUser), "A valid email or session id must be defined.");
    }
}