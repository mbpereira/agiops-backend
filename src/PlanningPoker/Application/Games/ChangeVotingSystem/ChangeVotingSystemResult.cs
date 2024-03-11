using PlanningPoker.Domain.Games;

namespace PlanningPoker.Application.Games.ChangeVotingSystem;

public record ChangeVotingSystemResult
{
    public string Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public IEnumerable<string> PossibleGrades { get; }
    public DateTime? UpdatedAt { get; }

    public ChangeVotingSystemResult(VotingSystem votingSystem)
    {
        Id = votingSystem.Id;
        Name = votingSystem.Name;
        Description = votingSystem.Description;
        PossibleGrades = votingSystem.GradeDetails.Values;
        UpdatedAt = votingSystem.UpdatedAtUtc;
    }
}