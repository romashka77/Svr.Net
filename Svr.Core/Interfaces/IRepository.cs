using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(long? id);
        T GetSingleBySpec(ISpecification<T> spec);
        IEnumerable<T> ListAll();
        IEnumerable<T> List(ISpecification<T> spec);
        T Add(T entity);
        int Add(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        bool EntityExists(long id);
        IQueryable<T> Table();
    }
}
