using EfCoreSoftDelete.Domain.Common.Interfaces;
using EfCoreSoftDelete.Domain.Entities;
using EfCoreSoftDelete.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreSoftDelete.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetSoftDeleteProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SetSoftDeleteProperties();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* E.g.
            modelBuilder.Entity<Car>().HasData(
                new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
                new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }

        private void SetSoftDeleteProperties()
        {
            var entities = ChangeTracker
                .Entries<ISoftDelete>()
                .Where(t => t.State == EntityState.Deleted)
                .ToArray();

            if (entities.Length == 0)
            {
                return;
            }

            foreach (var entry in entities)
            {
                var entity = entry.Entity;
                entity.SetDeleted(true);
                entry.State = EntityState.Modified;
                UpdateOwnedEntriesRecursive(entry);
            }

            return;

            void UpdateOwnedEntriesRecursive(EntityEntry entry)
            {
                var ownedReferencedEntries = entry.References
                    .Where(x => x.TargetEntry != null)
                    .Select(x => x.TargetEntry!)
                    .Where(x => x.State == EntityState.Deleted && x.Metadata.IsOwned());

                foreach (var ownedEntry in ownedReferencedEntries)
                {
                    ownedEntry.State = EntityState.Unchanged;
                    UpdateOwnedEntriesRecursive(ownedEntry);
                }

                var ownedCollectionEntries = entry.Collections
                    .Where(x => x.IsLoaded && x.CurrentValue != null)
                    .SelectMany(x => x.CurrentValue!.Cast<object>().Select(Entry))
                    .Where(x => x.State == EntityState.Deleted && x.Metadata.IsOwned());

                foreach (var ownedEntry in ownedCollectionEntries)
                {
                    ownedEntry.State = EntityState.Unchanged;
                    UpdateOwnedEntriesRecursive(ownedEntry);
                }
            }
        }
    }
}