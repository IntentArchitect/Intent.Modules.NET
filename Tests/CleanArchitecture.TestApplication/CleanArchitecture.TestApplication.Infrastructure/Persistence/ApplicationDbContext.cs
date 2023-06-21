using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.Async;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using CleanArchitecture.TestApplication.Domain.Entities.DefaultDiagram;
using CleanArchitecture.TestApplication.Domain.Entities.Enums;
using CleanArchitecture.TestApplication.Domain.Entities.Nullability;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Async;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.CRUD;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DDD;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.DefaultDiagram;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Enums;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations.Nullability;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }

        public DbSet<AggregateRoot> AggregateRoots { get; set; }
        public DbSet<AggregateRootLong> AggregateRootLongs { get; set; }
        public DbSet<AggregateSingleC> AggregateSingleCs { get; set; }
        public DbSet<AggregateTestNoIdReturn> AggregateTestNoIdReturns { get; set; }
        public DbSet<AsyncOperationsClass> AsyncOperationsClasses { get; set; }
        public DbSet<ClassWithDefault> ClassWithDefaults { get; set; }
        public DbSet<ClassWithEnums> ClassWithEnums { get; set; }
        public DbSet<CompositeManyB> CompositeManyBs { get; set; }
        public DbSet<CompositeSingleA> CompositeSingleAs { get; set; }
        public DbSet<CompositeSingleAA> CompositeSingleAAs { get; set; }
        public DbSet<CompositeSingleBB> CompositeSingleBBs { get; set; }
        public DbSet<DataContractClass> DataContractClasses { get; set; }
        public DbSet<ImplicitKeyAggrRoot> ImplicitKeyAggrRoots { get; set; }
        public DbSet<NullabilityPeer> NullabilityPeers { get; set; }
        public DbSet<TestNullablity> TestNullablities { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await DispatchEvents();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AccountHolderConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRootConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateRootLongConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateSingleCConfiguration());
            modelBuilder.ApplyConfiguration(new AggregateTestNoIdReturnConfiguration());
            modelBuilder.ApplyConfiguration(new AsyncOperationsClassConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithDefaultConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithEnumsConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeManyBConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleAAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeSingleBBConfiguration());
            modelBuilder.ApplyConfiguration(new DataContractClassConfiguration());
            modelBuilder.ApplyConfiguration(new ImplicitKeyAggrRootConfiguration());
            modelBuilder.ApplyConfiguration(new NullabilityPeerConfiguration());
            modelBuilder.ApplyConfiguration(new TestNullablityConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
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

        private async Task DispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }
    }
}