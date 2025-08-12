using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.SoftDelete.SoftDeleteEFCoreInterceptor", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence.Interceptors
{
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entries = eventData.Context.ChangeTracker
                .Entries<ISoftDelete>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var softDeletable in entries)
            {
                softDeletable.State = EntityState.Modified;
                softDeletable.Entity.SetDeleted(true);
                HandleDependencies(eventData.Context, softDeletable);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void HandleDependencies(DbContext context, EntityEntry entry)
        {
            var ownedReferencedEntries = entry.References
                .Where(x => x.TargetEntry != null)
                .Select(x => x.TargetEntry!)
                .Where(x => x.State == EntityState.Deleted && x.Metadata.IsOwned());

            foreach (var ownedEntry in ownedReferencedEntries)
            {
                ownedEntry.State = EntityState.Unchanged;
                HandleDependencies(context, ownedEntry);
            }

            var ownedCollectionEntries = entry.Collections
                .Where(x => x.IsLoaded && x.CurrentValue != null)
                .SelectMany(x => x.CurrentValue!.Cast<object>().Select(context.Entry))
                .Where(x => x.State == EntityState.Deleted && x.Metadata.IsOwned());

            foreach (var ownedEntry in ownedCollectionEntries)
            {
                ownedEntry.State = EntityState.Unchanged;
                HandleDependencies(context, ownedEntry);
            }
        }
    }
}