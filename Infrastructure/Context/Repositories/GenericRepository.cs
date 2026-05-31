using Application.Interfaces.Commons;
using Domain.Entities.Commons;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.QueryParameter;
using Shared.Results;
using System.Linq.Expressions;

namespace Infrastructure.Context.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public void Detach(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public virtual TEntity Add(TEntity entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual TEntity Delete(TEntity entity)
        {
            return _dbSet.Remove(entity).Entity;
        }
        public virtual TEntity Delete(TKey id)
        {
            var entity = _dbSet.Find(id);
            if (entity is null)
                throw new KeyNotFoundException($"Entity '{typeof(TEntity).Name}' with id '{id}' not found.");

            return _dbSet.Remove(entity).Entity;
        }

        public virtual void DeleteMulti(Expression<Func<TEntity, bool>> where)
        {
            var objects = _dbSet.Where(where).AsEnumerable();
            foreach (var obj in objects)
                _dbSet.Remove(obj);
        }
        public virtual TEntity SoftDelete(TKey id)
        {
            var entity = GetSingleById(id);

            if (entity is not BaseEntity baseEntity)
                throw new InvalidOperationException($"Entity '{typeof(TEntity).Name}' does not support soft delete.");

            baseEntity.IsDeleted = true;
            baseEntity.DeletedAt = DateTime.UtcNow;
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void DeleteMulti(IEnumerable<TEntity> where)
        {
            foreach (var obj in where)
                _dbSet.Remove(obj);
        }

        public async Task<bool> IsExistAsync(TKey id)
        {
            try
            {
                _ = await Task.FromResult(GetSingleById(id));
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public virtual TEntity GetSingleById(TKey id)
        {
            var entity = _dbSet.Find(id);
            if (entity is null || IsSoftDeleted(entity))
                throw new KeyNotFoundException($"Entity '{typeof(TEntity).Name}' with id '{id}' not found.");
            return entity;
        }

        private static bool IsSoftDeleted(TEntity entity) =>
            entity is BaseEntity { IsDeleted: true } ||
            entity is BaseEntity { DeletedAt: not null };

        // Kept from sample: convenience query method (includes string is currently unused)
        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where, string includes)
        {
            return _dbSet.Where(where).ToList();
        }

        public virtual TEntity GetSingleByCondition(Expression<Func<TEntity, bool>> expression, string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            var entity = query.FirstOrDefault(expression);
            if (entity is null)
                throw new KeyNotFoundException($"Entity '{typeof(TEntity).Name}' not found for the given condition.");
            return entity;
        }

        public virtual IEnumerable<TEntity> GetAll(string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return query;
        }

        public virtual IEnumerable<TEntity> GetMulti(Expression<Func<TEntity, bool>>? predicate, string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        // Kept from sample: no-tracking variant (not part of IGenericRepository today)
        public virtual IEnumerable<TEntity> GetMultiNoTracking(Expression<Func<TEntity, bool>>? predicate, string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (predicate != null)
                query = query.Where(predicate);

            return query;
        }

        public virtual IEnumerable<TEntity> GetMultiPaging(
            Expression<Func<TEntity, bool>> filter,
            out int total,
            int index = 0,
            int size = 50,
            string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            query = query.Where(filter);
            total = query.Count();

            return query
                .Skip(index * size)
                .Take(size);
        }

        public virtual IEnumerable<TEntity> GetMultiByFilterNoPaging(
            Expression<Func<TEntity, bool>>? predicate,
            GenericQueryParameters parameters,
            string[]? searchProperties,
            string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (predicate != null)
                query = query.Where(predicate);

            query = query.ApplyFilters(parameters.Filters);

            if (searchProperties != null && searchProperties.Length > 0)
                query = query.ApplySearch(parameters.Search, searchProperties);

            return query;
        }

        public virtual PagedResult<TEntity> GetPaged(GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null)
        {
            return GetPaged(null, parameters, searchProperties, includes);
        }

        public virtual PagedResult<TEntity> GetPaged(
            Expression<Func<TEntity, bool>>? predicate,
            GenericQueryParameters parameters,
            string[]? searchProperties,
            string[]? includes = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (predicate != null)
                query = query.Where(predicate);

            query = query.ApplyFilters(parameters.Filters);

            if (searchProperties != null && searchProperties.Length > 0)
                query = query.ApplySearch(parameters.Search, searchProperties);

            query = query.ApplySorting(parameters.SortBy, parameters.SortDirection);

            return query.ToPagedResult(parameters);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Count(where);
        }

        public virtual bool CheckContains(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        // Kept from sample: string-key lookup helper (not part of IGenericRepository today)
        public virtual TEntity GetSingleById(string id)
        {
            var entity = _dbSet.Find(id);
            if (entity is null || IsSoftDeleted(entity))
                throw new KeyNotFoundException($"Entity '{typeof(TEntity).Name}' with id '{id}' not found.");
            return entity;
        }
    }
}
