using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.EF.SqlServer.Application.Common.Interfaces;
using Entities.PrivateSetters.EF.SqlServer.Domain.Common.Interfaces;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Entities.PrivateSetters.EF.SqlServer.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly ICurrentUserService _currentUserService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Audited> Auditeds { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetAuditableFields();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SetAuditableFields();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AuditedConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
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
            new Car() { CarId = 3, Make = "Labourghini", Model = "Countach" });
            */
        }

        private void SetAuditableFields()
        {
            var userName = _currentUserService.UserId ?? throw new InvalidOperationException("UserId is null");
            var timestamp = DateTimeOffset.UtcNow;
            var entries = ChangeTracker.Entries().ToArray();

            foreach (var entry in entries)
            {
                if (entry.Entity is not IAuditable auditable)
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.SetCreated(userName, timestamp);
                        break;
                    case EntityState.Modified or EntityState.Deleted:
                        auditable.SetUpdated(userName, timestamp);
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}