﻿using PlanningPoker.Domain.Validation;

namespace PlanningPoker.UnitTests.Application.CreateGame
{
    public record CommandResult<TResponse> : BaseCommandResult
    {
        public TResponse? Data { get; private set; }

        protected CommandResult(TResponse? data, CommandStatus status, IEnumerable<Error> details) : base(status, details)
        {
            Data = data;
        }

        public static CommandResult<T> Success<T>(T data) =>
            new(data, CommandStatus.Success, Enumerable.Empty<Error>());

        public static CommandResult<T> Fail<T>(
            IEnumerable<Error> errors,
            CommandStatus status = CommandStatus.ValidationFailed) where T : class
                => new(null, status, errors);
    }

    public record BaseCommandResult
    {
        protected BaseCommandResult(CommandStatus status, IEnumerable<Error> details)
        {
            Status = status;
            Details = details;
        }

        public CommandStatus Status { get; private set; }

        public IEnumerable<Error> Details { get; private set; }

    }

    public record CommandResult : BaseCommandResult
    {
        protected CommandResult(CommandStatus status, IEnumerable<Error> errors)
            : base(status, errors)
        {
        }

        public static CommandResult Success() =>
            new CommandResult(
                CommandStatus.Success,
                Enumerable.Empty<Error>());

        public static CommandResult Fail(
            IEnumerable<Error> errors,
            CommandStatus status = CommandStatus.ValidationFailed) =>
                new CommandResult(status, errors);
    }
}