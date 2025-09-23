using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Application.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Common;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Common.Interfaces;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
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

        public DbSet<AppDbDomainPackageAuditLog> AppDbDomainPackageAuditLogs { get; set; }

        public DbSet<AppDbEntity> AppDbEntities { get; set; }
        public DbSet<DefaultDomainPackageAuditLog> DefaultDomainPackageAuditLogs { get; set; }
        public DbSet<DefaultEntity> DefaultEntities { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            await SetAuditableFieldsAsync();
            LogDiffAudit();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DispatchEventsAsync().GetAwaiter().GetResult();
            SetAuditableFieldsAsync().GetAwaiter().GetResult();
            LogDiffAudit();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AppDbDomainPackageAuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new AppDbEntityConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultDomainPackageAuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultEntityConfiguration());
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
                    .SelectMany(x => x.Entity.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }

        private async Task SetAuditableFieldsAsync()
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

            var userIdentifier = (await _currentUserService.GetAsync())?.Id ?? throw new InvalidOperationException("Id is null");
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

        private void LogDiffAudit()
        {
            var diffAuditEntries = ChangeTracker.Entries()
                .Where(entry => entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified &&
                                entry.Entity is IDiffAudit)
                .Select(entry => new
                {
                    entry.State,
                    entry.Entity,
                    entry.Properties
                })
                .ToArray();

            if (!diffAuditEntries.Any())
            {
                return;
            }

            var auditEntries = new List<DefaultDomainPackageAuditLog>();

            var userIdentifier = _currentUserService.UserId ?? throw new InvalidOperationException("UserId is null");
            var timestamp = DateTimeOffset.UtcNow;

            foreach (var entry in diffAuditEntries)
            {
                var entityName = entry.Entity.GetType().Name;
                var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntries.Add(new DefaultDomainPackageAuditLog
                        {
                            TableName = entityName,
                            Key = primaryKey?.CurrentValue?.ToString(),
                            ColumnName = "EntityCreated",
                            OldValue = null,
                            NewValue = null,
                            ChangedBy = userIdentifier,
                            ChangedDate = timestamp,
                        });

                        foreach (var prop in entry.Properties)
                        {
                            auditEntries.Add(new DefaultDomainPackageAuditLog
                            {
                                TableName = entityName,
                                Key = primaryKey?.CurrentValue?.ToString(),
                                ColumnName = prop.Metadata.Name,
                                OldValue = null,
                                NewValue = prop.CurrentValue?.ToString(),
                                ChangedBy = userIdentifier,
                                ChangedDate = timestamp,
                            });
                        }
                        break;
                    case EntityState.Deleted:
                        auditEntries.Add(new DefaultDomainPackageAuditLog
                        {
                            TableName = entityName,
                            Key = primaryKey?.CurrentValue?.ToString(),
                            ColumnName = "EntityDeleted",
                            OldValue = null,
                            NewValue = null,
                            ChangedBy = userIdentifier,
                            ChangedDate = timestamp,
                        });
                        break;
                    case EntityState.Modified:
                        foreach (var prop in entry.Properties)
                        {
                            if (!Equals(prop.OriginalValue, prop.CurrentValue))
                            {
                                auditEntries.Add(new DefaultDomainPackageAuditLog
                                {
                                    TableName = entityName,
                                    Key = primaryKey?.CurrentValue?.ToString(),
                                    ColumnName = prop.Metadata.Name,
                                    OldValue = prop.OriginalValue?.ToString(),
                                    NewValue = prop.CurrentValue?.ToString(),
                                    ChangedBy = userIdentifier,
                                    ChangedDate = timestamp,
                                });
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            DefaultDomainPackageAuditLogs.AddRange(auditEntries);
        }
    }
}