namespace PlanningPoker.Application.Abstractions
{
    public abstract class CommandHandler<TCommand>
    {
        public abstract Task<CommandResult> HandleAsync(TCommand request);
    }

    public abstract class CommandHandler<TCommand, TCommandResult> where TCommandResult : class
    {
        public abstract Task<CommandResult<TCommandResult>> HandleAsync(TCommand command);
    }
}
