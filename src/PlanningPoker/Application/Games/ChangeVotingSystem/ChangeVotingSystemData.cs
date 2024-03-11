namespace PlanningPoker.Application.Games.ChangeVotingSystem;

public record ChangeVotingSystemData(
    string? Name = null,
    IList<string>? PossibleGrades = null,
    string? Description = null);