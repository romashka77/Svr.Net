using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T>, IAsyncRepository<T> where T : BaseEntity
    {
        private readonly DataContext dbContext;
        private DbSet<T> _entities;

        #region Ctor
        public EfRepository(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #endregion
        #region Methods
        public virtual T GetById(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.SingleOrDefault(m => m.Id == id);
        }

        public virtual T GetSingleBySpec(ISpecification<T> spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }
            return List(spec).FirstOrDefault();
        }


        public virtual async Task<T> GetByIdAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.SingleOrDefaultAsync(m => m.Id == id);
        }

        public virtual IEnumerable<T> ListAll()
        {
            return Entities.AsEnumerable();
        }

        public virtual async Task<List<T>> ListAllAsync()
        {
            return await Entities.ToListAsync();
        }

        public virtual IEnumerable<T> List(ISpecification<T> spec)
        {
            // получение запроса, который включает в себя все выражения includes
            var queryableResultWithIncludes = spec.Includes.Aggregate(Entities.AsQueryable(), (current, include) => current.Include(include));

            // измените IQueryable, чтобы включить любые строковые операторы include
            var secondaryResult = spec.IncludeStrings.Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            // возвращает результат запроса с помощью выражения критериев спецификации
            return secondaryResult.Where(spec.Criteria).AsEnumerable();
        }
        public virtual async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            // получение запроса, который включает в себя все выражения includes
            var queryableResultWithIncludes = spec.Includes.Aggregate(Entities.AsQueryable(), (current, include) => current.Include(include));

            // измените IQueryable, чтобы включить любые строковые операторы include
            var secondaryResult = spec.IncludeStrings.Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            // возвращает результат запроса с помощью выражения критериев спецификации
            return await secondaryResult.Where(spec.Criteria).ToListAsync();
        }

        public virtual T Add(T entity)
        {
            //try
            //{
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Entities.Add(entity);
            dbContext.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException dbEx)
            //{
            //    //ensure that the detailed error text is saved in the Log
            //    throw new Exception(GetFullErrorTextAndRollbackEntityChanges(dbEx), dbEx);
            //}
            return entity;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Entities.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual int Add(IEnumerable<T> entities)
        {
            //try
            //{
            int i = 0;
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities)
            {
                Entities.Add(entity);
                i++;
            }
            dbContext.SaveChanges();
            return i;
            //}
            //catch (DbEntityValidationException dbEx)
            //{
            //    //ensure that the detailed error text is saved in the Log
            //    throw new Exception(GetFullErrorTextAndRollbackEntityChanges(dbEx), dbEx);
            //}
        }
        public virtual async Task<int> AddAsync(IEnumerable<T> entities)
        {
            int i = 0;
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities)
            {
                await Entities.AddAsync(entity);
                i++;
            }
            await dbContext.SaveChangesAsync();
            return i;
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Entities.Remove(entity);
            //dbContext.Entry(entity).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }
        public virtual async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            //Entities.Remove(entity);
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }
        public virtual bool EntityExists(long id) => Entities.Any(e => e.Id == id);
        public virtual async Task<bool> EntityExistsAsync(long id) => await Entities.AnyAsync(e => e.Id == id);
        /// <summary>
        /// Получить таблицу
        /// </summary>
        public virtual IQueryable<T> Table() { return Entities; }
        #endregion

        #region Properties
        /// <summary>
        /// Возвращает таблицу с включенной функцией "без отслеживания" (функция EF) используйте ее только при загрузке записей только для операций только для чтения
        /// </summary>
        public virtual IQueryable<T> TableNoTracking { get { return Entities.AsNoTracking(); } }

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = dbContext.Set<T>();
                return _entities;
            }
        }
        #endregion
    }
}
