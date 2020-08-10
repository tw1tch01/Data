using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Data.Common;
using Data.Contexts;
using Data.Extensions;
using Data.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Data.IntegrationTests")]

namespace Data.Repositories
{
    public class EntityRepository<TContext> : IEntityRepository<TContext> where TContext : IAuditedContext
    {
        #region Fields

        private readonly TContext _dataContext;
        private readonly ILogger<IAuditedContext> _logger;
        private readonly int _retryAttempts;

        #endregion Fields

        #region Constructor

        public EntityRepository(TContext dataContext, ILogger<IAuditedContext> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
            _retryAttempts = 3;
        }

        #endregion Constructor

        #region Methods

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _dataContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync<TEntity>(ICollection<TEntity> entities) where TEntity : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) throw new ArgumentException($"Cannot add an empty collection. (Parameter '{nameof(entities)}')");

            await _dataContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dataContext.Set<TEntity>().Attach(entity);
        }

        public void AttachRange<TEntity>(ICollection<TEntity> entities) where TEntity : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) throw new ArgumentException($"Cannot attach an empty collection. (Parameter '{nameof(entities)}')");

            _dataContext.Set<TEntity>().AttachRange(entities);
        }

        public async Task<TEntity> FindByPrimaryKeyAsync<TEntity, TProperty>(TProperty primaryKey) where TEntity : class
        {
            if (primaryKey == null) throw new ArgumentNullException(nameof(primaryKey));

            return await _dataContext.Set<TEntity>().FindAsync(primaryKey);
        }

        public async Task<TEntity> GetAsync<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return await GetQueryable(specification).FirstOrDefaultAsync();
        }

        public async Task<TEntity> SingleAsync<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return await GetQueryable(specification).SingleOrDefaultAsync();
        }

        public async Task<ICollection<TEntity>> ListAsync<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return await GetQueryable(specification).ToListAsync();
        }

        public async Task<PagedCollection<TEntity>> PagedListAsync<TEntity>(int page, int pageSize, LinqSpecification<TEntity> specification, Expression<Func<TEntity, dynamic>> primaryKeyExpression = null) where TEntity : class
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            Task<int> totalRecordsResolver(IQueryable<TEntity> query) => primaryKeyExpression == null ? query.CountAsync() : query.Select(primaryKeyExpression).CountAsync();
            Func<IQueryable<TEntity>, IQueryable<TEntity>> paginationResolverFunction() => a => a.Skip(page * pageSize).Take(pageSize);

            var totalRecords = await QueryAsync(specification, totalRecordsResolver);
            var pagedEntities = await ListAsync(specification.AddQuery(paginationResolverFunction()));

            return new PagedCollection<TEntity>(page, pageSize, totalRecords, pagedEntities);
        }

        public async Task<TResult> QueryAsync<TEntity, TResult>(LinqSpecification<TEntity> specification, Func<IQueryable<TEntity>, Task<TResult>> resolver) where TEntity : class
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            var query = GetQueryable(specification);

            return await resolver(query);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _dataContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange<TEntity>(ICollection<TEntity> entities) where TEntity : class
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) throw new ArgumentException($"Cannot remove an empty collection. (Parameter '{nameof(entities)}')");

            _dataContext.Set<TEntity>().RemoveRange(entities);
        }

        public async Task<int> SaveAsync()
        {
            return await _dataContext.ConcurrencySave(_retryAttempts, _logger, default);
        }

        #endregion Methods

        #region Private Methods

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class
        {
            var setQueryable = _dataContext.Set<TEntity>().AsQueryable();

            if (specification.IsTracked) setQueryable.AsNoTracking();

            setQueryable = specification.ApplyFilters(setQueryable);
            setQueryable = specification.ApplyModifiers(setQueryable);

            return setQueryable;
        }

        #endregion Private Methods
    }
}