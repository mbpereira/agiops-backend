using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Users
{
    public class User : AggregateRoot<User>
    {
        public string Name { get; private set; }
        public Email? Email { get; private set; }
        public Guest? Guest { get; private set; }

        public bool IsGuest => Guest is not null;

        private User(EntityId id, string name, string email) : base(EntityId.AutoIncrement())
        {
            Name = name;
            IdentifyUser(email);
        }

        private User(string name, string email) : this(EntityId.AutoIncrement(), name, email)
        {
            Name = name;
            IdentifyUser(email);
        }

        private User(string name)
            : base(EntityId.AutoIncrement())
        {
            Name = name;
            Email = null;
            Guest = new Guest(Guid.NewGuid().ToString());
        }

        protected override void ConfigureValidationRules(Validator<User> validator)
        {
            if (Guest is not null)
                validator.RuleFor(u => u.Email!.Value)
                    .EmailAddress();
            
            if (Email is not null)
                validator.RuleFor(u => u.Guest)
                     .Null();

            validator.RuleFor(u => u.Name)
                .NotEmpty()
                .MinimumLength(3);
        }

        private void IdentifyUser(string email)
        {
            Email = new Email(email);
            Guest = null;
        }

        public static User New(string name, string email) => new(name, email);

        public static User NewGuest(string name) => new(name);

        public static User Load(EntityId id, string name, string email) => new(id, name, email);
    }
}
