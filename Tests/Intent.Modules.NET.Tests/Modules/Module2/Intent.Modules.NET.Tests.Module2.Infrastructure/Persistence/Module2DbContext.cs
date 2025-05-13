using Intent.Modules.NET.Tests.Module2.Application.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Common;
using Intent.Modules.NET.Tests.Module2.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Infrastructure.Persistence
{
    public class Module2DbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;
        public Module2DbContext(DbContextOptions<Module2DbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<MyCustomer> MyCustomers { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DispatchEventsAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new MyCustomerConfiguration());
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
    }
}