using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Interfaces;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Infrastructure.Persistence.Configurations;
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

        public DbSet<AggregateRoot> AggregateRoots { get; set; }
        public DbSet<AggregateRootLong> AggregateRootLongs { get; set; }
        public DbSet<AggregateSingleC> AggregateSingleCs { get; set; }
        public DbSet<AggregateTestNoIdReturn> AggregateTestNoIdReturns { get; set; }
        public DbSet<CompositeManyB> CompositeManyBs { get; set; }
        public DbSet<CompositeSingleA> CompositeSingleAs { get; set; }
        public DbSet<CompositeSingleAA> CompositeSingleAAs { get; set; }
        public DbSet<CompositeSingleBB> CompositeSingleBBs { get; set; }
        public DbSet<ImplicitKeyAggrRoot> ImplicitKeyAggrRoots { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await DispatchEvents();
            return await base.SaveChangesAsync(cancellationToken);
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
            modelBuilder.ApplyConfiguration(new ImplicitKeyAggrRootConfiguration());
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