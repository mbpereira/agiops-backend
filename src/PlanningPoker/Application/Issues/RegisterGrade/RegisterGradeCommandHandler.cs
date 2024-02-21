using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommandHandler(IUnitOfWork uow, IUserContext authenticationContext)
        : ICommandHandler<RegisterGradeCommand>
    {
        public async Task<CommandResult> HandleAsync(RegisterGradeCommand command)
        {
            if (!command.IsValid)
                return CommandResult.Fail(command.Errors, CommandStatus.ValidationFailed);

            var issue = await uow.Issues.GetByIdAsync(command.IssueId);

            if (issue is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            var userInformation = await authenticationContext.GetCurrentUserAsync();

            issue.RegisterGrade(userInformation.Id, command.Grade);

            if (!issue.IsValid)
                return CommandResult.Fail(issue.Errors, CommandStatus.ValidationFailed);

            await uow.Issues.ChangeAsync(issue);
            await uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
