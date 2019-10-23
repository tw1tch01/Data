using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DevGate.Data.Contexts;
using DevGate.Data.Extensions;
using DevGate.Data.Other;
using DevGate.Data.Specifications;
using DevGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Repositories
{
	/// <summary>
	/// See <see cref="IEntityRepository{TContext}"/>
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public class EntityRepository<TContext> : IEntityRepository<TContext> where TContext : IDbContext
	{
		#region Properties

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.AuditFields"/>
		/// </summary>
		public AuditFields AuditFields { get; }

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Logger"/>
		/// </summary>
		public ILogger Logger { get; }

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.RetryAttempts"/>
		/// </summary>
		public int RetryAttempts { get; }

		/// <summary>
		/// Context instance
		/// </summary>
		protected TContext DataContext { get; }

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Main Constructor
		/// </summary>
		public EntityRepository(TContext context, ILogger<TContext> logger)
		{
			DataContext = context;
			Logger = logger;
			RetryAttempts = 3;
			AuditFields = new AuditFields();
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.AddAsync{TEntity}(ICollection{TEntity})"/>
		/// </summary>
		public async Task<IEntityRepository<TContext>> AddAsync<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity
		{
			await DataContext.Set<TEntity>().AddRangeAsync(entities);
			return this;
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.AddAsync{TEntity}(TEntity)"/>
		/// </summary>
		public async Task<IEntityRepository<TContext>> AddAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
		{
			await DataContext.Set<TEntity>().AddAsync(entity);
			return this;
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Attach{TEntity}(TEntity)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Attach<TEntity>(TEntity entity) where TEntity : BaseEntity
		{
			DataContext.Set<TEntity>().Attach(entity);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Attach{TEntity}(ICollection{TEntity})"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Attach<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity
		{
			DataContext.Set<TEntity>().AttachRange(entities);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.FindAsync{TEntity}(Specification{TEntity})"/>
		/// </summary>
		public async Task<TEntity> FindAsync<TEntity, TProperty>(TProperty primaryKey) where TEntity : BaseEntity
		{
			if (primaryKey == null || primaryKey == default) throw new ArgumentNullException(nameof(primaryKey));

			return await DataContext.Set<TEntity>().FindAsync(primaryKey);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.GetAsync{TEntity}(Specification{TEntity})"/>
		/// </summary>
		public async Task<TEntity> GetAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await specification.AsQueryable(DataContext).FirstOrDefaultAsync();
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.ListAsync{TEntity}(Specification{TEntity})"/>
		/// </summary>
		public async Task<ICollection<TEntity>> ListAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await specification.AsQueryable(DataContext).ToListAsync();
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.QueryAsync{TEntity, TResult}(Specification{TEntity}, Func{IQueryable{TEntity}, Task{TResult}})"/>
		/// </summary>
		public async Task<TResult> QueryAsync<TEntity, TResult>(Specification<TEntity> specification, Func<IQueryable<TEntity>, Task<TResult>> resolver) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await resolver(specification.AsQueryable(DataContext));
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Remove{TEntity}(TEntity)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
		{
			DataContext.Set<TEntity>().Remove(entity);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Remove{TEntity}(ICollection{TEntity})"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Remove<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity
		{
			DataContext.Set<TEntity>().RemoveRange(entities.Where(e => e.IsDeletable()));
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Restore{TEntity}(TEntity)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Restore<TEntity>(TEntity entity) where TEntity : NonDeletableEntity
		{
			entity.Restore();
			return Attach(entity);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Restore{TEntity}(ICollection{TEntity})"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Restore<TEntity>(ICollection<TEntity> entities) where TEntity : NonDeletableEntity
		{
			Array.ForEach(entities.ToArray(), (entity) => entity.Restore());
			return Attach(entities);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.SaveAsync"/>
		/// </summary>
		public async Task<int> SaveAsync()
		{
			var validationErrors = DataContext.GetValidationErrors();
			if (validationErrors.Any()) throw new ValidationException($"Validation errors:\n {string.Join(Environment.NewLine, validationErrors)}");

			DataContext.SetAuditingFields();

			return await DataContext.ConcurrencySave(RetryAttempts, Logger, true, default);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.SingleAsync{TEntity}(Specification{TEntity})"/>
		/// </summary>
		public async Task<TEntity> SingleAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await specification.AsQueryable(DataContext).SingleAsync();
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Update{TEntity}(TEntity)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Update<TEntity>(TEntity entity) where TEntity : BaseEntity
		{
			DataContext.Set<TEntity>().Update(entity);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Update{TEntity}(ICollection{TEntity})"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Update<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity
		{
			DataContext.Set<TEntity>().UpdateRange(entities);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		#endregion Methods
	}
}