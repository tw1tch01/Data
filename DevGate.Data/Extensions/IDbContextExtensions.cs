using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevGate.Data.Contexts;
using DevGate.Domain.Entities;
using DevGate.Domain.Entities.Audits;
using DevGate.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace DevGate.Data.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="IDbContext"/>
	/// </summary>
	public static class IDbContextExtensions
	{
		#region Fields

		private static readonly EntityState[] _entityStates = new[] {
			EntityState.Added,
			EntityState.Deleted,
			EntityState.Modified
		};

		#endregion Fields

		/// <summary>
		/// Asynchronously saves all changes in this context to the database with with retries in the event of concurrency failure.
		/// </summary>
		/// <param name="context">Context instance</param>
		/// <param name="retryAttempts">Retry attempts</param>
		/// <param name="logger">Logger instance</param>
		/// <param name="acceptAllChangesOnSuccess"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>Number of affected records</returns>
		public static async Task<int> ConcurrencySave(this IDbContext context, int retryAttempts, ILogger logger, bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
		{
			bool successfulSave;
			var attempts = 1;
			do
			{
				try
				{
					return await context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
				}
				catch (DbUpdateConcurrencyException exception) when (attempts <= retryAttempts)
				{
					logger.LogWarning(exception, $"Attempt {attempts}/{retryAttempts} failed. Retrying...");
					successfulSave = false;
					attempts++;
				}
			} while (!successfulSave);

			throw new InvalidOperationException("A serious issue has been encountered! Contact support immediately.");
		}

		/// <summary>
		/// Gets all validation errors
		/// </summary>
		/// <param name="context">Context instances</param>
		/// <returns>Dictionary of entity with validation errors</returns>
		public static IDictionary<object, ICollection<ValidationResult>> GetValidationErrors(this IDbContext context)
		{
			var validationErrors = new Dictionary<object, ICollection<ValidationResult>>();
			context.ChangeTracker.Entries().Where(entry => _entityStates.Contains(entry.State)).ToList().ForEach(entry =>
			{
				var ValidationResult = new List<ValidationResult>();

				if (Validator.TryValidateObject(entry.Entity, new ValidationContext(entry.Entity), ValidationResult, true)) return;

				validationErrors.Add(entry.Entity, ValidationResult);
			});

			return validationErrors;
		}

		/// <summary>
		/// Set the res[ective entitys' audit fields
		/// </summary>
		/// <param name="context">Context instance</param>
		public static void SetAuditingFields(this IDbContext context)
		{
			if (context.UserScope == null) throw new InvalidOperationException($"{nameof(context.UserScope)} has not bet set. Cannot set audit fields in {nameof(IDbContext)}");

			context.ChangeTracker.Entries().Where(entry => _entityStates.Contains(entry.State)).ToList().ForEach(entry =>
			{
				switch (entry.State)
				{
					case EntityState.Added:
						SetAddedAuditFields(entry.Entity as ICreatedAudit, context.UserScope.Username);
						break;

					case EntityState.Deleted:
						SetDeletedAuditFields(entry, context.UserScope.Username);
						break;

					case EntityState.Modified:
						SetModifiedAuditFields(entry.Entity as IModifiedAudit, context.UserScope.Username);
						break;
				}
			});
		}

		#region Private Methods

		private static void SetAddedAuditFields(ICreatedAudit createdAudit, string username)
		{
			createdAudit?.SetAuditFields(username, DateTime.UtcNow);
		}

		private static void SetDeletedAuditFields(EntityEntry entry, string username)
		{
			if (entry.Entity as NonDeletableEntity == null) return;

			entry.State = EntityState.Modified;
			(entry.Entity as NonDeletableEntity).SetAuditFields(username, DateTime.UtcNow);
		}

		private static void SetModifiedAuditFields(IModifiedAudit modifiedAudit, string username)
		{
			modifiedAudit?.SetAuditFields(username, DateTime.UtcNow);
		}

		#endregion Private Methods
	}
}