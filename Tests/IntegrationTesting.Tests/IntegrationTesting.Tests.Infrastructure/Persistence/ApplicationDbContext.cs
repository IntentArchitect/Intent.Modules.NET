using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using IntegrationTesting.Tests.Domain.Common;
using IntegrationTesting.Tests.Domain.Common.Interfaces;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Infrastructure.Persistence.Configurations;
using IntegrationTesting.Tests.Infrastructure.Persistence.Configurations.Converters;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace IntegrationTesting.Tests.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<BadSignatures> BadSignatures { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DiffId> DiffIds { get; set; }
        public DbSet<DiffPk> DiffPks { get; set; }
        public DbSet<DtoReturn> DtoReturns { get; set; }
        public DbSet<HasDateOnlyField> HasDateOnlyFields { get; set; }
        public DbSet<HasMissingDep> HasMissingDeps { get; set; }
        public DbSet<MissingDep> MissingDeps { get; set; }
        public DbSet<NoReturn> NoReturns { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<PartialCrud> PartialCruds { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RichProduct> RichProducts { get; set; }
        public DbSet<UniqueConVal> UniqueConVals { get; set; }

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
            modelBuilder.ApplyConfiguration(new BadSignaturesConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new ChildConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DiffIdConfiguration());
            modelBuilder.ApplyConfiguration(new DiffPkConfiguration());
            modelBuilder.ApplyConfiguration(new DtoReturnConfiguration());
            modelBuilder.ApplyConfiguration(new HasDateOnlyFieldConfiguration());
            modelBuilder.ApplyConfiguration(new HasMissingDepConfiguration());
            modelBuilder.ApplyConfiguration(new MissingDepConfiguration());
            modelBuilder.ApplyConfiguration(new NoReturnConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ParentConfiguration());
            modelBuilder.ApplyConfiguration(new PartialCrudConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new RichProductConfiguration());
            modelBuilder.ApplyConfiguration(new UniqueConValConfiguration());
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

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties(typeof(DateTimeOffset)).HaveConversion(typeof(UtcDateTimeOffsetConverter));
        }
    }
}