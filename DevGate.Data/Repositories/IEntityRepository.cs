using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevGate.Data.Contexts;
using DevGate.Data.Other;
using DevGate.Data.Specifications;
using DevGate.Domain.Entities;
using DevGate.Domain.Entities.Audits;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Repositories
{
	/// <summary>
	/// Main repository for performing operations on the entities inside the context
	/// </summary>
	/// <typeparam name="TContext"><see cref="IDbContext"/> type</typeparam>
	public interface IEntityRepository<TContext> where TContext : IDbContext
	{
		#region Properties

		/// <summary>
		/// Information related to  Audit Fields types and their respective property names
		/// </summary>
		AuditFields AuditFields { get; }

		/// <summary>
		/// Logger
		/// </summary>
		ILogger Logger { get; }

		/// <summary>
		/// Number of retry attemps to save after optimistic concurreny fails
		/// </summary>
		int RetryAttempts { get; }

		#endregion Properties

		#region Method

		/// <summary>
		/// Adds the collection of entites to the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entities">Etnity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> AddAsync<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity;

		/// <summary>
		/// Adds the collection of entites to the <see cref="IDbContext"/> and sets their audit fields
		/// </summary>
		/// <typeparam name="TEntity">Entity types</typeparam>
		/// <param name="entities">Collection of entity objects</param>
		/// <param name="createdBy"><see cref="ICreated.CreatedBy"/></param>
		/// <param name="createdOn"><see cref="ICreated.CreatedOn"/></param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> AddAsync<TEntity>(ICollection<TEntity> entities, string createdBy, DateTime createdOn) where TEntity : BaseEntity, ICreated;

		/// <summary>
		/// Adds the entity into the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> AddAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

		/// <summary>
		/// Adds the entity into the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <param name="createdBy"><see cref="ICreated.CreatedBy"/></param>
		/// <param name="createdOn"><see cref="ICreated.CreatedOn"/></param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> AddAsync<TEntity>(TEntity entity, string createdBy, DateTime createdOn) where TEntity : BaseEntity, ICreated;

		/// <summary>
		/// Attach the entities to the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Attach<TEntity>(TEntity entity) where TEntity : BaseEntity;

		/// <summary>
		/// Attach the entities to the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entities">Collection of entity objects</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Attach<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity;

		/// <summary>
		/// Find the entity based on the <see cref="Specification{TEntity}"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="specification">Entity specification</param>
		/// <returns>If entity found, returns an object of the entity, else null.</returns>
		Task<TEntity> FindAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity;

		/// <summary>
		/// Returns a collection of entties based on the <see cref="Specification{TEntity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="specification">Entity specification</param>
		/// <returns>Collection of the entities if found, else an empty collection</returns>
		Task<ICollection<TEntity>> ListAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity;

		/// <summary>
		/// Retrieves a list of entities from the <see cref="IDbContext"/> based on the query <see cref="Specification{TEntity}"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <typeparam name="TResult">Result type</typeparam>
		/// <param name="specification">Entity specification</param>
		/// <param name="parameters">Query paramaters</param>
		/// <returns>Parameter results</returns>
		Task<TResult> QueryAsync<TEntity, TResult>(Specification<TEntity> specification, Func<IQueryable<TEntity>, Task<TResult>> parameters) where TEntity : BaseEntity;

		/// <summary>
		/// Remove an entity from the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;

		/// <summary>
		/// Soft deletes the <see cref="NonDeletableEntity"/> by setting the property <see cref="NonDeletableEntity.IsDeleted"/> true, and sets the audit fields
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <param name="deletedBy"><see cref="NonDeletableEntity.DeletedBy"/></param>
		/// <param name="deletedOn"><see cref="NonDeletableEntity.DeletedOn"/></param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Remove<TEntity>(TEntity entity, string deletedBy, DateTime deletedOn) where TEntity : NonDeletableEntity;

		/// <summary>
		/// Removes a collection of entities from the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entities">Collection of entity objects</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Remove<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity;

		/// <summary>
		/// Soft deletes a collection of <see cref="NonDeletableEntity"/> by setting the property <see cref="NonDeletableEntity.IsDeleted"/> true, and sets the audit fields
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entities">Collection of entity objects</param>
		/// <param name="deletedBy"><see cref="NonDeletableEntity.DeletedBy"/></param>
		/// <param name="deletedOn"><see cref="NonDeletableEntity.DeletedOn"/></param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Remove<TEntity>(ICollection<TEntity> entities, string deletedBy, DateTime deletedOn) where TEntity : NonDeletableEntity;

		/// <summary>
		/// Restore a soft deleted entity by setting <see cref="NonDeletableEntity.DeletedBy"/> and <see cref="NonDeletableEntity.DeletedOn"/> to null
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Restore<TEntity>(TEntity entity) where TEntity : NonDeletableEntity;

		/// <summary>
		/// Restore a collection of soft deleted entities by setting their <see cref="NonDeletableEntity.DeletedBy"/> and <see cref="NonDeletableEntity.DeletedOn"/> to null
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Restore<TEntity>(ICollection<TEntity> entities) where TEntity : NonDeletableEntity;

		/// <summary>
		/// Saves all changes made ot the context
		/// </summary>
		/// <returns>Number of records affected</returns>
		Task<int> SaveAsync();

		/// <summary>
		/// Returns a single entity based on the <see cref="Specification{TEntity}"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="specification">Entity specification</param>
		/// <returns>Entity object</returns>
		Task<TEntity> SingleAsync<TEntity>(Specification<TEntity> specification) where TEntity : BaseEntity;

		/// <summary>
		/// Attach an entity to this context
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Update<TEntity>(TEntity entity) where TEntity : BaseEntity;

		/// <summary>
		/// Attach an entity to this context
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entity">Entity object</param>
		/// <param name="updatedBy"><see cref="IUpdated.UpdatedBy"/></param>
		/// <param name="updatedOn"><see cref="IUpdated.UpdatedOn"/></param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Update<TEntity>(TEntity entity, string updatedBy, DateTime updatedOn) where TEntity : BaseEntity, IUpdated;

		/// <summary>
		/// Attaches the collection of altered entities to the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entities">Collection of entity objects</param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Update<TEntity>(ICollection<TEntity> entities) where TEntity : BaseEntity;

		/// <summary>
		/// Attaches and updates the entity to the <see cref="IDbContext"/>
		/// </summary>
		/// <typeparam name="TEntity">Entity type</typeparam>
		/// <param name="entities">Collection of entity objects</param>
		/// <param name="updatedBy"><see cref="IUpdated.UpdatedBy"/></param>
		/// <param name="updatedOn"><see cref="IUpdated.UpdatedOn"/></param>
		/// <returns>Repository instance</returns>
		Task<IEntityRepository<TContext>> Update<TEntity>(ICollection<TEntity> entities, string updatedBy, DateTime updatedOn) where TEntity : BaseEntity, IUpdated;

		#endregion Method
	}
}