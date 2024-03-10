#region

using PlanningPoker.Application.Abstractions.Commands;

#endregion

namespace PlanningPoker.Application.Games.CreateVotingSystem;

public class CreateVotingSystemCommand(string name, IList<string> possibleGrades, string? description = null) : Command
{
    public string Name { get; } = name;
    public IList<string> PossibleGrades { get; } = possibleGrades;
    public string? Description { get; } = description;
}