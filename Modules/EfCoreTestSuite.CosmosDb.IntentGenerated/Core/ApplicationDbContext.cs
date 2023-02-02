using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core.Associations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core.Inheritance;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core.InheritanceAssociations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core.NestedComposition;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Core.Polymorphic;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        public DbSet<AbstractBaseClass> AbstractBaseClasses { get; set; }
        public DbSet<AbstractBaseClassAssociated> AbstractBaseClassAssociateds { get; set; }
        public DbSet<Associated> Associateds { get; set; }
        public DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public DbSet<Base> Bases { get; set; }
        public DbSet<BaseAssociated> BaseAssociateds { get; set; }
        public DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        public DbSet<ClassA> ClassAs { get; set; }
        public DbSet<Composite> Composites { get; set; }
        public DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        public DbSet<ConcreteBaseClassAssociated> ConcreteBaseClassAssociateds { get; set; }
        public DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public DbSet<Derived> Deriveds { get; set; }
        public DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        public DbSet<DerivedClassForAbstractAssociated> DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        public DbSet<DerivedClassForConcreteAssociated> DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public DbSet<ExplicitKeyClass> ExplicitKeyClasses { get; set; }
        public DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        public DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        public DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        public DbSet<ImplicitKeyClass> ImplicitKeyClasses { get; set; }
        public DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        public DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        public DbSet<K_SelfReference> K_SelfReferences { get; set; }
        public DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }
        public DbSet<Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<Poly_ConcreteA> Poly_ConcreteAs { get; set; }
        public DbSet<Poly_ConcreteB> Poly_ConcreteBs { get; set; }
        public DbSet<Poly_SecondLevel> Poly_SecondLevels { get; set; }
        public DbSet<WeirdClass> WeirdClasses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new A_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new AbstractBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new AbstractBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new AssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new BaseConfiguration());
            modelBuilder.ApplyConfiguration(new BaseAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new C_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new ClassAConfiguration());
            modelBuilder.ApplyConfiguration(new CompositeConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new D_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new D_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new E_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new ExplicitKeyClassConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new G_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new H_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new H_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new ImplicitKeyClassConfiguration());
            modelBuilder.ApplyConfiguration(new J_MultipleAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new J_RequiredDependentConfiguration());
            modelBuilder.ApplyConfiguration(new K_SelfReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new M_SelfReferenceBiNavConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new WeirdClassConfiguration());
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

        /// <summary>
        /// Calling EnsureCreatedAsync is necessary to create the required containers and insert the seed data if present in the model. 
        /// However EnsureCreatedAsync should only be called during deployment, not normal operation, as it may cause performance issues.
        /// </summary>
        public async Task EnsureDbCreatedAsync()
        {
            await Database.EnsureCreatedAsync();
        }
    }
}