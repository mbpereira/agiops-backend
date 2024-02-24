#region

using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.Domain.Games;

public sealed record UserGrade
{
    internal UserGrade(EntityId userId, string grade)
    {
        UserId = userId;
        Grade = grade;
    }

    public EntityId UserId { get; private set; }
    public string Grade { get; private set; }
}