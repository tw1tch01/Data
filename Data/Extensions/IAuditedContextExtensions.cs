using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Data.UnitTests")]

namespace Data.Extensions
{
    public static class IAuditedContextExtensions
    {
        private static readonly EntityState[] _entityStates = new[] {
            EntityState.Added,
            EntityState.Modified
        };

        public static IDictionary<object, ICollection<ValidationResult>> GetValidationErrors(this IAuditedContext context)
        {
            var validationErrors = new Dictionary<object, ICollection<ValidationResult>>();
            context.ChangeTracker.Entries().Where(entry => _entityStates.Contains(entry.State)).ToList().ForEach(entry =>
            {
                var ValidationResult = new List<ValidationResult>();

                if (!Validator.TryValidateObject(entry.Entity, new ValidationContext(entry.Entity), ValidationResult, true))
                    validationErrors.Add(entry.Entity, ValidationResult);
            });

            return validationErrors;
        }

        public static void SetAuditingFields(this IAuditedContext context)
        {
            if (context.ContextScope == null) throw new InvalidOperationException($"{nameof(context.ContextScope)} has not bet set. Cannot set audit fields in {nameof(IAuditedContext)}");

            foreach (var stateAction in context.ContextScope.StateActions) InvokeStateActionPerEntity(context.ChangeTracker.Entries().Where(entry => entry.State == stateAction.Key).ToList(), stateAction.Value);
        }

        public static async Task<int> ConcurrencySave(this IAuditedContext context, int retryAttempts, ILogger logger, CancellationToken cancellationToken)
        {
            var validationErrors = context.GetValidationErrors();
            if (validationErrors.Any()) throw new ValidationException($"Validation errors:\n {string.Join(Environment.NewLine, validationErrors)}");

            context.SetAuditingFields();

            bool successfulSave;
            var currentAttempt = 1;

            do
            {
                try
                {
                    return await context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException exception) when (currentAttempt <= retryAttempts)
                {
                    logger.LogWarning(exception, $"Attempt {currentAttempt}/{retryAttempts} failed. Retrying...");
                    successfulSave = false;
                    currentAttempt++;
                }
            } while (!successfulSave);

            throw new InvalidOperationException("A serious issue has been encountered! Contact support immediately.");
        }

        internal static void InvokeStateActionPerEntity(ICollection<EntityEntry> entities, Action<EntityEntry> auditAction)
        {
            foreach (var entity in entities) auditAction.Invoke(entity);
        }
    }
}