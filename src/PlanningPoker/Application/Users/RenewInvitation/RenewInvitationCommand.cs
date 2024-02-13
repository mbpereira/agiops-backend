using FluentValidation;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.RenewInvitation
{
    public class RenewInvitationCommand : Command<RenewInvitationCommand>
    {
        public int Id { get; private set; }

        public RenewInvitationCommand(int id)
        {
            Id = id;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<RenewInvitationCommand> validator)
        {
            validator.CreateRuleFor(c => c.Id)
                .GreaterThan(0);
        }
    }
}
