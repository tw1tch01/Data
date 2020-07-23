using System.Threading;
using System.Threading.Tasks;
using Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.Contexts
{
    public interface IAuditedContext
    {
        ChangeTracker ChangeTracker { get; }
        ContextScope ContextScope { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}