using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.CosmosDb.IntentGenerated.DependencyInjection;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IOptions<DbContextConfiguration> _dbContextConfig;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<DbContextConfiguration> dbContextConfig) : base(options)
        {
            _dbContextConfig = dbContextConfig;
        }

        public DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }

        public DbSet<Associated> Associateds { get; set; }
        public DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public DbSet<BaseAssociated> BaseAssociateds { get; set; }
        public DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        public DbSet<ClassA> ClassAs { get; set; }
        public DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public DbSet<Derived> Deriveds { get; set; }
        public DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        public DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        public DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        public DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        public DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        public DbSet<K_SelfReference> K_SelfReferences { get; set; }
        public DbSet<L_SelfReferenceMultiple> L_SelfReferenceMultiples { get; set; }
        public DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);

            if (!string.IsNullOrWhiteSpace(_dbContextConfig.Value?.DefaultContainerName))
            {
                modelBuilder.HasDefaultContainer(_dbContextConfig.Value?.DefaultContainerName);
            }
            modelBuilder.ApplyConfiguration(new A_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new AssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new BaseAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new C_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new ClassAConfiguration());
            modelBuilder.ApplyConfiguration(new D_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new D_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new DerivedConfiguration());
            modelBuilder.ApplyConfiguration(new E_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new G_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new H_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new H_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new J_MultipleAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new J_RequiredDependentConfiguration());
            modelBuilder.ApplyConfiguration(new K_SelfReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new L_SelfReferenceMultipleConfiguration());
            modelBuilder.ApplyConfiguration(new M_SelfReferenceBiNavConfiguration());
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
        /// If configured to do so, a check is performed to see
        /// whether the database exist and if not will create it
        /// based on this container configuration.
        /// </summary>
        public void EnsureDbCreated()
        {
            if (_dbContextConfig.Value.EnsureDbCreated == true)
            {
                Database.EnsureCreated();
            }
        }
    }
}