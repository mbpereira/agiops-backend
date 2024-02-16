using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;

namespace PlanningPoker.Domain.Users
{
    public sealed class User : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public Email? Email { get; private set; }
        public Guest? Guest { get; private set; }

        public bool IsGuest => Guest is not null;

        private User(int id, string name, string? email = null, string? sessionId = null) : base(id)
        {
            SetName(name);
            IdentifyUser(email, sessionId);
        }

        public void SetName(string name)
        {
            if (!name.HasMinLength(minLength: 3))
            {
                AddError(UserErrors.InvalidName);
                return;
            }

            Name = name;
        }

        public void IdentifyUser(string? email, string? sessionId)
        {
            if (email.IsEmail())
            {
                Email = new Email(email!);
                Guest = null;
                return;
            }

            if (sessionId is not null)
            {
                Guest = new Guest(sessionId!);
                return;
            }

            AddError(UserErrors.InvalidIdentification);
        }

        public static User Load(int id, string name, string? email, string? sessionId) => new(id, name, email, sessionId);
        public static User New(string name, string email) => new(EntityId.AutoIncrement(), name, email, sessionId: null);
        public static User NewGuest(string name) => new(EntityId.AutoIncrement(), name, email: null, sessionId: Guid.NewGuid().ToString());
    }
}
