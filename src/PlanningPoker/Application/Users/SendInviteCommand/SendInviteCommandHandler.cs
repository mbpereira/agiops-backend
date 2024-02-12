using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.SendInviteCommand
{
    public class SendInviteCommandHandler : ICommandHandler<SendInviteCommand>
    {
        private readonly ITenantContext _tenantContext;
        private readonly IUnitOfWork _uow;

        public SendInviteCommandHandler(IUnitOfWork uow, ITenantContext tenantContext)
        {
            _uow = uow;
            _tenantContext = tenantContext;
        }

        public async Task<CommandResult> HandleAsync(SendInviteCommand command)
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var invite = Invite.New(tenant.Id, command.To, command.Role);

            var validationResult = invite.Validate();

            if (!validationResult.Success)
                return CommandResult.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            await _uow.Invites.AddAsync(invite);
            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
