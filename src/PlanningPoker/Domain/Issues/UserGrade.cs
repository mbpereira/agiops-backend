using Domain.Abstractions;

namespace Domain.Issues
{
    public sealed record UserGrade(EntityId UerId, decimal Grade);
}
