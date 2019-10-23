using System.Threading;
using System.Threading.Tasks;
using DevGate.Data.Other;
using DevGate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DevGate.Data.Contexts
{
	/// <summary>
	/// Represents an context controlling interface for <see cref="DbContext"/>
	/// </summary>
	public interface IDbContext
	{
		/// <summary>
		/// Provides information and operations for the entity instances this context is tracking.
		/// </summary>
		ChangeTracker ChangeTracker { get; }

		/// <summary>
		/// See <see cref="UserContextScope"/>
		/// </summary>
		UserContextScope UserScope { get; }

		/// <summary>
		/// Saves the <see cref="DbContext" /> changes
		/// </summary>
		/// <param name="acceptAllChangesOnSuccess"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates a <see cref="DbSet{TEntity}"/> to query and save instances of TEntity. Implement as a `new` method which returns the base <see cref="DbContext.Set{TEntity}"/> method
		/// </summary>
		/// <typeparam name="TEntity">Entity type in the set</typeparam>
		/// <returns>Set of given entity type</returns>
		DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
	}
}