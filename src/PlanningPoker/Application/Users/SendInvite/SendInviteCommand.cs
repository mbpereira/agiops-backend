using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.SendInvite
{
    public class SendInviteCommand : Command<SendInviteCommand>
    {
        public string To { get; private set; }
        public Role Role { get; private set; }

        public SendInviteCommand(string to, Role role)
        {
            To = to;
            Role = role;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<SendInviteCommand> validator)
        {
        }
    }
}