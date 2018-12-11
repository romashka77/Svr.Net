﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(long? id);
        Task<List<T>> ListAllAsync();
        Task<List<T>> ListAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        Task<int> AddAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> EntityExistsAsync(long id);
    }
}
