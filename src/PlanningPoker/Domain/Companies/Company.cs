using Domain.Abstractions;
using Domain.Validation;
using FluentValidation;

namespace Domain.Companys
{
    public sealed class Company : Entity<Company>
    {
        public string Name { get; private set; }
        public int OwnerId { get; private set; }
        public string? Domain { get; private set; }

        private Company(string name, int ownerId, string? domain = null)
            : this(id: Constants.AutoIncrement, name, ownerId, domain)
        {
        }

        private Company(int id, string name, int ownerId, string? domain = null)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            Domain = domain;

        }

        protected override void ConfigureValidationRules(Validator<Company> validator)
        {
            validator.RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(3);

            validator.RuleFor(c => c.OwnerId)
                .GreaterThan(0);

            if (!string.IsNullOrEmpty(Domain))
            {
                validator.RuleFor(c => c.Domain)
                    .NotEmpty()
                    .MinimumLength(3);
            }
        }

        public static Company New(string name, int ownerId, string? domain = null)
            => new(name, ownerId, domain);

        public static Company Load(int id, string name, int ownerId, string? domain = null)
            => new(id, name, ownerId, domain);

        public void ChangeName(string name) => Name = name;

        public void OwnDomain(string domain) => Domain = domain;

        public void ChangeOwner(int ownerId) => OwnerId = ownerId;
    }
}
