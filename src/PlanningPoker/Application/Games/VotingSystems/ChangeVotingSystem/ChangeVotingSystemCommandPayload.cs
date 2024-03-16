namespace PlanningPoker.Application.Games.VotingSystems.ChangeVotingSystem;

public record ChangeVotingSystemCommandPayload(
    string? Name = null,
    IList<string>? PossibleGrades = null,
    string? Description = null);