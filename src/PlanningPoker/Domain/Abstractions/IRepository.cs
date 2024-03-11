namespace PlanningPoker.Domain.Abstractions;

public interface IRepository<T>
{
    Task<T> AddAsync(T entity);
    Task<T> AddAsync(IList<T> entities);
    Task<T?> GetByIdAsync(EntityId id);
    Task ChangeAsync(T issue);
    Task RemoveAsync(T entity);
}