using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Issues
{
    public static class GameErrors
    {
        public static Error InvalidVotingSystem = new(nameof(Game), nameof(Game.SetVotingSystem), "Provided voting system is not valid.");
        public static Error OwnerAlreadySet = new(nameof(Game), nameof(Game.UserId), "You cannot change the game owner, as it has already been set.");
        public static Error InvalidUserId = Error.GreaterThan(nameof(Game), nameof(Game.UserId), value: 0);
        public static Error InvalidName = Error.MinLength(nameof(Game), nameof(Game.Name), minLength: 1);
        public static Error InvalidPassword = Error.MinLength(nameof(Game), nameof(Game.Credentials.Password), minLength: 6);
    }
}
