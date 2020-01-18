﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Common;
using Data.Contexts;
using Data.Specifications;

namespace Data.Repositories
{
    public interface IContextRepository<TContext> where TContext : IAuditedContext
    {
        Task<IContextRepository<TContext>> AddAsync<TEntity>(TEntity entity) where TEntity : class;

        Task<IContextRepository<TContext>> AddRangeAsync<TEntity>(ICollection<TEntity> entities) where TEntity : class;

        IContextRepository<TContext> Attach<TEntity>(TEntity entity) where TEntity : class;

        IContextRepository<TContext> AttachRange<TEntity>(ICollection<TEntity> entities) where TEntity : class;

        Task<TEntity> FindByPrimaryKeyAsync<TEntity, TProperty>(TProperty primaryKey) where TEntity : class;

        Task<TEntity> GetAsync<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class;

        Task<TEntity> SingleAsync<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class;

        Task<ICollection<TEntity>> ListAsync<TEntity>(LinqSpecification<TEntity> specification) where TEntity : class;

        Task<PagedCollection<TEntity>> PagedListAsync<TEntity>(int page, int pageSize, LinqSpecification<TEntity> specification, Expression<Func<TEntity, dynamic>> primaryKeyExpression = null) where TEntity : class;

        Task<TResult> QueryAsync<TEntity, TResult>(LinqSpecification<TEntity> specification, Func<IQueryable<TEntity>, Task<TResult>> resolver) where TEntity : class;

        IContextRepository<TContext> Remove<TEntity>(TEntity entity) where TEntity : class;

        IContextRepository<TContext> RemoveRange<TEntity>(ICollection<TEntity> entities) where TEntity : class;

        Task<int> SaveAsync();
    }
}