using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<CorporateFuneralCoverQuote> CorporateFuneralCoverQuotes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<FuneralCoverQuote> FuneralCoverQuotes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<User> Users { get; set; }

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
            modelBuilder.ApplyConfiguration(new CorporateFuneralCoverQuoteConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new FileUploadConfiguration());
            modelBuilder.ApplyConfiguration(new FuneralCoverQuoteConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
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