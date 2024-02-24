namespace PlanningPoker.Application.Abstractions.Commands;

public enum CommandStatus
{
    Success = 1,
    ValidationFailed = 2,
    RecordNotFound = 3
}