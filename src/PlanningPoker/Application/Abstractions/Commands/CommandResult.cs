﻿using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Abstractions.Commands
{
    public record CommandResult<TResponse> : BaseCommandResult where TResponse : class
    {
        public TResponse? Data { get; private set; }

        private CommandResult(TResponse? data, CommandStatus status, IEnumerable<Error> details) : base(status,
            details)
        {
            Data = data;
        }

        public static CommandResult<TResponse> Success(TResponse data) =>
            new(data, CommandStatus.Success, Enumerable.Empty<Error>());

        public static CommandResult<TResponse> Fail(
            IEnumerable<Error> errors,
            CommandStatus status = CommandStatus.ValidationFailed)
            => new(null, status, errors);

        public static CommandResult<TResponse> Fail(
            CommandStatus status = CommandStatus.ValidationFailed)
            => new(null, status, Enumerable.Empty<Error>());
    }

    public record CommandResult : BaseCommandResult
    {
        protected CommandResult(CommandStatus status, IEnumerable<Error> errors)
            : base(status, errors)
        {
        }

        public static CommandResult Success() =>
            new(CommandStatus.Success, Enumerable.Empty<Error>());

        public static CommandResult Fail(
            IEnumerable<Error> errors,
            CommandStatus status) => new(status, errors);

        public static CommandResult Fail(CommandStatus status) =>
            new(status, Enumerable.Empty<Error>());
    }

    public abstract record BaseCommandResult
    {
        protected BaseCommandResult(CommandStatus status, IEnumerable<Error> details)
        {
            Status = status;
            Details = details;
        }

        public CommandStatus Status { get; private set; }

        public IEnumerable<Error> Details { get; private set; }
    }
}