using Domain.Shared;
using Domain.Validation;
using FluentValidation;

namespace Domain.Companys
{
    public class Company : BaseEntity<Company>
    {
        public string Name { get; private set; }
        public int OwnerId { get; private set; }
        public string? Domain { get; private set; }

        private Company(string name, int ownerId, string? domain = null)
        {
            Name = name;
            OwnerId = ownerId;
            Domain = domain;
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            Validator.RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(3);
            Validator.RuleFor(c => c.OwnerId)
                .GreaterThan(0);

            if (!string.IsNullOrEmpty(Domain))
            {
                Validator.RuleFor(c => c.Domain)
                    .NotEmpty()
                    .MinimumLength(3);
            }
        }

        private Company(int id, string name, int ownerId, string? domain = null)
            : this(name, ownerId, domain)
        {
            Id = id;
        }

        public static Company New(string name, int ownerId, string? domain = null)
            => new(name, ownerId, domain);

        public static Company Load(int id, string name, int ownerId, string? domain = null)
            => new(id, name, ownerId, domain);

        public void ChangeName(string name) => Name = name;

        public void OwnDomain(string domain) => Domain = domain;

        public void ChangeOwner(int ownerId) => OwnerId = ownerId;

        public override ValidationResult Validate()
            => new(Validator.Validate(this));
    }
}
