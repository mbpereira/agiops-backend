using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Domain.Abstractions
{
    public interface IRepository<T>
    {
        Task<T> AddAsync(T entity);
        Task<T?> GetByIdAsync(EntityId id);
        Task ChangeAsync(T issue);

    }
}
