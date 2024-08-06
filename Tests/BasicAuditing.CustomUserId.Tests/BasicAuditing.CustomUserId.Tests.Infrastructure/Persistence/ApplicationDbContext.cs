using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BasicAuditing.CustomUserId.Tests.Application.Common.Interfaces;
using BasicAuditing.CustomUserId.Tests.Domain.Common;
using BasicAuditing.CustomUserId.Tests.Domain.Common.Interfaces;
using BasicAuditing.CustomUserId.Tests.Domain.Entities;
using BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService) : base(options)
        {
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
        }

        public DbSet<Customer> Customers { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            SetAuditableFields();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DispatchEventsAsync().GetAwaiter().GetResult();
            SetAuditableFields();
            return base.SaveChanges(acceptAllChangesOnSuccess);
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
            /* Eg.
            
            modelBuilder.Entity<Car>().HasData(
                new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
                new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }

        private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }

        private void SetAuditableFields()
        {
            var auditableEntries = ChangeTracker.Entries()
                .Where(entry => entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified &&
                                entry.Entity is IAuditable)
                .Select(entry => new
                {
                    entry.State,
                    Property = new Func<string, PropertyEntry>(entry.Property),
                    Auditable = (IAuditable)entry.Entity
                })
                .ToArray();

            if (!auditableEntries.Any())
            {
                return;
            }

            var userIdentifier = _currentUserService.UserId ?? throw new InvalidOperationException("UserId is null");
            var timestamp = DateTimeOffset.UtcNow;

            foreach (var entry in auditableEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Auditable.SetCreated(userIdentifier, timestamp);
                        break;
                    case EntityState.Modified or EntityState.Deleted:
                        entry.Auditable.SetUpdated(userIdentifier, timestamp);
                        entry.Property("CreatedBy").IsModified = false;
                        entry.Property("CreatedDate").IsModified = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}