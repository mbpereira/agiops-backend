﻿#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Application.Abstractions.Commands;

public record CommandResult<TResponse>(TResponse? Payload, CommandStatus Status, IEnumerable<Error> Errors)
    : BaseCommandResult(Status, Errors) where TResponse : class
{
    public static CommandResult<TResponse> Success(TResponse payload)
    {
        return new CommandResult<TResponse>(payload, CommandStatus.Success, Enumerable.Empty<Error>());
    }

    public static CommandResult<TResponse> Fail(
        IEnumerable<Error> errors,
        CommandStatus status)
    {
        return new CommandResult<TResponse>(null, status, errors);
    }

    public static CommandResult<TResponse> Fail(
        CommandStatus status)
    {
        return new CommandResult<TResponse>(null, status, Enumerable.Empty<Error>());
    }

    public static implicit operator CommandResult<TResponse>((IEnumerable<Error>, CommandStatus) tuple)
    {
        return Fail(tuple.Item1, tuple.Item2);
    }

    public static implicit operator CommandResult<TResponse>(TResponse payload)
    {
        return Success(payload);
    }

    public static implicit operator CommandResult<TResponse>(CommandStatus status)
    {
        return Fail(status);
    }
}

public record CommandResult(CommandStatus Status, IEnumerable<Error> Errors) : BaseCommandResult(Status, Errors)
{
    public static CommandResult Success()
    {
        return new CommandResult(CommandStatus.Success, Enumerable.Empty<Error>());
    }

    public static CommandResult Fail(
        IEnumerable<Error> errors,
        CommandStatus status)
    {
        return new CommandResult(status, errors);
    }

    public static CommandResult Fail(CommandStatus status)
    {
        return new CommandResult(status, Enumerable.Empty<Error>());
    }

    public static implicit operator CommandResult((IEnumerable<Error>, CommandStatus) tuple)
    {
        return Fail(tuple.Item1, tuple.Item2);
    }

    public static implicit operator CommandResult(CommandStatus status)
    {
        return Fail(status);
    }
}

public abstract record BaseCommandResult(CommandStatus Status, IEnumerable<Error> Errors);