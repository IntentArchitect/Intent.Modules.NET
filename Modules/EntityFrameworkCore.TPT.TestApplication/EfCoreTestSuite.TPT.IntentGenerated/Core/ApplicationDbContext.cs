using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.TPT.IntentGenerated.DomainEvents;
using EfCoreTestSuite.TPT.IntentGenerated.Entities;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.InheritanceAssociations;
using EfCoreTestSuite.TPT.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreTestSuite.TPT.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDomainEventService _domainEventService;

        public DbSet<AbstractBaseClass> AbstractBaseClasses { get; set; }

        [IntentManaged(Mode.Ignore)]
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _domainEventService = new DomainEventService();
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }
        public DbSet<AbstractBaseClassAssociated> AbstractBaseClassAssociateds { get; set; }
        public DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        public DbSet<ConcreteBaseClassAssociated> ConcreteBaseClassAssociateds { get; set; }
        public DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        public DbSet<DerivedClassForAbstractAssociated> DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        public DbSet<DerivedClassForConcreteAssociated> DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<FkAssociatedClass> FkAssociatedClasses { get; set; }
        public DbSet<FkBaseClass> FkBaseClasses { get; set; }
        public DbSet<FkBaseClassAssociated> FkBaseClassAssociateds { get; set; }
        public DbSet<FkDerivedClass> FkDerivedClasses { get; set; }
        public DbSet<Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<Poly_ConcreteA> Poly_ConcreteAs { get; set; }
        public DbSet<Poly_ConcreteB> Poly_ConcreteBs { get; set; }

        public DbSet<Poly_RootAbstract> Poly_RootAbstracts { get; set; }
        public DbSet<Poly_RootAbstract_Aggr> Poly_RootAbstract_Aggrs { get; set; }

        public DbSet<Poly_RootAbstract_Comp> Poly_RootAbstract_Comps { get; set; }
        public DbSet<Poly_SecondLevel> Poly_SecondLevels { get; set; }
        public DbSet<Poly_TopLevel> Poly_TopLevels { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await DispatchEvents();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new AbstractBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new AbstractBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_RootAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_RootAbstract_CompConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_TopLevelConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Customize Default Schema
            // modelBuilder.HasDefaultSchema("EntityFrameworkCore.TPT.TestApplication");

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