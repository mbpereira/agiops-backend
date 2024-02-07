using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.UnitTests.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuthenticationContext _authenticationContext;

        public RegisterGradeCommandHandler(IUnitOfWork uow, IAuthenticationContext authenticationContext)
        {
            _uow = uow;
            _authenticationContext = authenticationContext;
        }

        public async Task<CommandResult> HandleAsync(RegisterGradeCommand command)
        {
            var validationResult = command.Validate();

            if (!validationResult.Success)
                return CommandResult.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var issue = await _uow.Issues.GetByIdAsync(command.IssueId);

            if (issue is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            var userInformation = await _authenticationContext.GetCurrentUserAsync();

            issue.RegisterGrade(userInformation?.Id ?? 0, command.Grade);

            await _uow.Issues.ChangeAsync(issue);
            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
