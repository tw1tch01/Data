using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevGate.Data.Contexts;
using DevGate.Domain.Entities;
using DevGate.Domain.Entities.Audits;
using DevGate.Data.Other;
using DevGate.Data.Specifications;
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
		/// See <see cref="IEntityRepository{TContext}.AddAsync{TEntity}(ICollection{TEntity}, string, DateTime)"/>
		/// </summary>
		public async Task<IEntityRepository<TContext>> AddAsync<TEntity>(ICollection<TEntity> entities, string createdBy, DateTime createdOn) where TEntity : BaseEntity, ICreated
		{
			Array.ForEach(entities.ToArray(), (entity) => entity.Create(createdBy, createdOn));
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
		/// See <see cref="IEntityRepository{TContext}.AddAsync{TEntity}(TEntity, string, DateTime)"/>
		/// </summary>
		public async Task<IEntityRepository<TContext>> AddAsync<TEntity>(TEntity entity, string createdBy, DateTime createdOn) where TEntity : BaseEntity, ICreated
		{
			entity.Create(createdBy, createdOn);
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
		public async Task<TEntity> FindAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await ConvertToQuery(specification).FirstOrDefaultAsync();
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.ListAsync{TEntity}(Specification{TEntity})"/>
		/// </summary>
		public async Task<ICollection<TEntity>> ListAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await ConvertToQuery(specification).ToListAsync();
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.QueryAsync{TEntity, TResult}(Specification{TEntity}, Func{IQueryable{TEntity}, Task{TResult}})"/>
		/// </summary>
		public async Task<TResult> QueryAsync<TEntity, TResult>(Specification<TEntity> specification, Func<IQueryable<TEntity>, Task<TResult>> resolver) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await resolver(ConvertToQuery(specification));
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Remove{TEntity}(TEntity)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Remove<TEntity>(TEntity entity) where TEntity : BaseEntity
		{
			if (!entity.IsDeletable()) return Remove(entity as NonDeletableEntity, nameof(EntityRepository<TContext>), DateTime.UtcNow);

			DataContext.Set<TEntity>().Remove(entity);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Remove{TEntity}(TEntity, string, DateTime)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Remove<TEntity>(TEntity entity, string deletedBy, DateTime deletedOn) where TEntity : NonDeletableEntity
		{
			entity.Delete(deletedBy, deletedOn);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Remove{TEntity}(ICollection{TEntity})"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Remove<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity
		{
			if (entities.Any(e => !e.IsDeletable())) Remove(entities.Where(e => !e.IsDeletable()) as ICollection<NonDeletableEntity>, nameof(EntityRepository<TContext>), DateTime.UtcNow);

			DataContext.Set<TEntity>().RemoveRange(entities.Where(e => e.IsDeletable()));
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Remove{TEntity}(ICollection{TEntity}, string, DateTime)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Remove<TEntity>(ICollection<TEntity> entities, string deletedBy, DateTime deletedOn) where TEntity : NonDeletableEntity
		{
			Array.ForEach(entities.ToArray(), (entity) => entity.Delete(deletedBy, deletedOn));
			DataContext.Set<TEntity>().RemoveRange(entities.Where(e => e.IsDeletable()));
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.SaveAsync"/>
		/// </summary>
		public async Task<int> SaveAsync()
		{
			return await DataContext.SaveChangesAsync();
		}

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.SingleAsync{TEntity}(Specification{TEntity})"/>
		/// </summary>
		public async Task<TEntity> SingleAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));

			return await ConvertToQuery(specification).SingleAsync();
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
		/// See <see cref="IEntityRepository{TContext}.Update{TEntity}(TEntity, string, DateTime)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Update<TEntity>(TEntity entity, string updatedBy, DateTime updatedOn) where TEntity : BaseEntity, IUpdated
		{
			entity.Update(updatedBy, updatedOn);
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

		/// <summary>
		/// See <see cref="IEntityRepository{TContext}.Update{TEntity}(ICollection{TEntity}, string, DateTime)"/>
		/// </summary>
		public Task<IEntityRepository<TContext>> Update<TEntity>(ICollection<TEntity> entities, string updatedBy, DateTime updatedOn) where TEntity : BaseEntity, IUpdated
		{
			Array.ForEach(entities.ToArray(), (entity) => entity.Update(updatedBy, updatedOn));
			DataContext.Set<TEntity>().UpdateRange(entities);
			return Task.FromResult<IEntityRepository<TContext>>(this);
		}

		#endregion Methods

		#region Private Methods

		private IQueryable<TEntity> ConvertToQuery<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity
		{
			var query = DataContext.Set<TEntity>().AsQueryable();

			if (specification.AsNoTracking) query = query.AsNoTracking();

			query = specification.Filter(query);
			query = specification.Modify(query);

			return query;
		}

		#endregion Private Methods
	}
}