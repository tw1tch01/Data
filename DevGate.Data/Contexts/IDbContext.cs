using System.Threading;
using System.Threading.Tasks;
using DevGate.Data.Entities;
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
		/// Saves the <see cref="DbContext" /> changes
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates a <see cref="DbSet{TEntity}"/> to query and save instances of TEntity. />
		/// </summary>
		/// <typeparam name="TEntity">Entity type in the set</typeparam>
		/// <returns>Set of given entity type</returns>
		DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
	}
}