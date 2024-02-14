﻿using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommandHandler : ICommandHandler<RegisterGradeCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserContext _authenticationContext;

        public RegisterGradeCommandHandler(IUnitOfWork uow, IUserContext authenticationContext)
        {
            _uow = uow;
            _authenticationContext = authenticationContext;
        }

        public async Task<CommandResult> HandleAsync(RegisterGradeCommand command)
        {
            if (!command.IsValid)
                return CommandResult.Fail(command.Errors, CommandStatus.ValidationFailed);

            var issue = await _uow.Issues.GetByIdAsync(command.IssueId);

            if (issue is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            var userInformation = await _authenticationContext.GetCurrentUserAsync();

            issue.RegisterGrade(userInformation.Id, command.Grade);

            if (!issue.IsValid)
                return CommandResult.Fail(issue.Errors, CommandStatus.ValidationFailed);

            await _uow.Issues.ChangeAsync(issue);
            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
