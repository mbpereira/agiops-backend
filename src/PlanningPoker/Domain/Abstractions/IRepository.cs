﻿using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Domain.Abstractions
{
    public interface IRepository<T>
    {
        Task<Game> AddAsync(T entity);
    }
}
