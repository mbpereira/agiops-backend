#region

using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.Application.Games.VotingSystems.ChangeVotingSystem;

public record ChangeVotingSystemResult
{
    public ChangeVotingSystemResult(VotingSystem votingSystem)
    {
        Id = votingSystem.Id;
        Name = votingSystem.Name;
        Description = votingSystem.Description;
        PossibleGrades = votingSystem.GradeDetails.Values;
        UpdatedAtUtc = votingSystem.UpdatedAtUtc;
    }

    public string Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public IEnumerable<string> PossibleGrades { get; }
    public DateTime? UpdatedAtUtc { get; }
}