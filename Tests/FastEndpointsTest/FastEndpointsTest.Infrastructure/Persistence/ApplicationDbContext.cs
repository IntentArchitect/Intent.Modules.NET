using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Common.Interfaces;
using FastEndpointsTest.Domain.Entities.CRUD;
using FastEndpointsTest.Domain.Entities.DDD;
using FastEndpointsTest.Domain.Entities.Enums;
using FastEndpointsTest.Domain.Entities.Pagination;
using FastEndpointsTest.Domain.Entities.UniqueIndexConstraint;
using FastEndpointsTest.Infrastructure.Persistence.Configurations.CRUD;
using FastEndpointsTest.Infrastructure.Persistence.Configurations.DDD;
using FastEndpointsTest.Infrastructure.Persistence.Configurations.Enums;
using FastEndpointsTest.Infrastructure.Persistence.Configurations.Pagination;
using FastEndpointsTest.Infrastructure.Persistence.Configurations.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace FastEndpointsTest.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<AggregateRoot> AggregateRoots { get; set; }
        public DbSet<AggregateRootLong> AggregateRootLongs { get; set; }
        public DbSet<AggregateSingleC> AggregateSingleCs { get; set; }
        public DbSet<AggregateTestNoIdReturn> AggregateTestNoIdReturns { get; set; }
        public DbSet<CompositeManyB> CompositeManyBs { get; set; }
        public DbSet<CompositeSingleA> CompositeSingleAs { get; set; }
        public DbSet<CompositeSingleAA> CompositeSingleAAs { get; set; }
        public DbSet<CompositeSingleBB> CompositeSingleBBs { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DataContractClass> DataContractClasses { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ClassWithEnums> ClassWithEnums { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<PersonEntry> PersonEntries { get; set; }
        public DbSet<AggregateWithUniqueConstraintIndexElement> AggregateWithUniqueConstraintIndexElements { get; set; }
        public DbSet<AggregateWithUniqueConstraintIndexStereotype> AggregateWithUniqueConstraintIndexStereotypes { get; set; }

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
            modelBuilder.ApplyConfiguration(new AggregateRootConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRootLongConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateSingleCConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateTestNoIdReturnConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeManyBConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleBBConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AccountHolderConfiguration());
            modelBuilder.ApplyConfiguration(new CameraConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DataContractClassConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithEnumsConfiguration());
            modelBuilder.ApplyConfiguration(new LogEntryConfiguration());
            modelBuilder.ApplyConfiguration(new PersonEntryConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateWithUniqueConstraintIndexElementConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateWithUniqueConstraintIndexStereotypeConfiguration());
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