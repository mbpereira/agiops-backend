using Domain.Abstractions;

namespace Domain.Issues
{
    public record UserGrade(EntityId userId, int grade);
}
