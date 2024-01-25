using FluentValidation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public sealed class User : AggregateRoot<User>
    {
        public string Name { get; private set; }
        public Email? Email { get; private set; }
        public Guest? Guest { get; private set; }

        public bool IsGuest => Guest is not null;

        private User(EntityId id, string name, string? email) : base(id)
        {
            Name = name;
            IdentifyUser(email);
        }

        protected override void ConfigureValidationRules(IValidationHandler<User> validator)
        {
            validator.CreateRuleFor(u => u.Email!.Value)
                .EmailAddress()
                .When(u => u.Guest is null);

            validator.CreateRuleFor(u => u.Guest)
                 .Null()
                 .When(u => u.Email is not null);

            validator.CreateRuleFor(u => u.Name)
                .NotEmpty()
                .MinimumLength(3);
        }

        private void IdentifyUser(string? email)
        {
            if (email is not null)
            {
                Email = new Email(email);
                Guest = null;
                return;
            }

            Guest = new Guest(Guid.NewGuid().ToString());
        }

        public static User New(string name, string email) => new(EntityId.AutoIncrement(), name, email);

        public static User NewGuest(string name) => new(EntityId.AutoIncrement(), name, email: null);

        public static User Load(int id, string name, string email) => new(new EntityId(id), name, email);
    }
}
