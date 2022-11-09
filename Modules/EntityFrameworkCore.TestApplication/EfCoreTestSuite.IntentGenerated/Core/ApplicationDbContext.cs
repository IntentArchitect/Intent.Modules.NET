using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using EfCoreTestSuite.IntentGenerated.Entities.Indexes;
using EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        public DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        public DbSet<ComplexDefaultIndex> ComplexDefaultIndices { get; set; }
        public DbSet<CustomIndex> CustomIndices { get; set; }
        public DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public DbSet<DefaultIndex> DefaultIndices { get; set; }
        public DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public DbSet<E2_RequiredCompositeNav> E2_RequiredCompositeNavs { get; set; }
        public DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public DbSet<FK_A_CompositeForeignKey> FK_A_CompositeForeignKeys { get; set; }
        public DbSet<FK_B_CompositeForeignKey> FK_B_CompositeForeignKeys { get; set; }
        public DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        public DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        public DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        public DbSet<Inhabitant> Inhabitants { get; set; }
        public DbSet<Internode> Internodes { get; set; }
        public DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        public DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        public DbSet<K_SelfReference> K_SelfReferences { get; set; }
        public DbSet<L_SelfReferenceMultiple> L_SelfReferenceMultiples { get; set; }
        public DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }
        public DbSet<PK_A_CompositeKey> PK_A_CompositeKeys { get; set; }
        public DbSet<PK_B_CompositeKey> PK_B_CompositeKeys { get; set; }
        public DbSet<PK_PrimaryKeyInt> PK_PrimaryKeyInts { get; set; }
        public DbSet<PK_PrimaryKeyLong> PK_PrimaryKeyLongs { get; set; }
        public DbSet<StereotypeIndex> StereotypeIndices { get; set; }
        public DbSet<Texture> Textures { get; set; }
        public DbSet<Tree> Trees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new A_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new C_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new ComplexDefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new CustomIndexConfiguration());
            modelBuilder.ApplyConfiguration(new D_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new D_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new E_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new E2_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new FK_A_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new FK_B_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new G_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new H_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new H_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new InhabitantConfiguration());
            modelBuilder.ApplyConfiguration(new InternodeConfiguration());
            modelBuilder.ApplyConfiguration(new J_MultipleAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new J_RequiredDependentConfiguration());
            modelBuilder.ApplyConfiguration(new K_SelfReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new L_SelfReferenceMultipleConfiguration());
            modelBuilder.ApplyConfiguration(new M_SelfReferenceBiNavConfiguration());
            modelBuilder.ApplyConfiguration(new PK_A_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_B_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyIntConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyLongConfiguration());
            modelBuilder.ApplyConfiguration(new StereotypeIndexConfiguration());
            modelBuilder.ApplyConfiguration(new TextureConfiguration());
            modelBuilder.ApplyConfiguration(new TreeConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Customize Default Schema
            // modelBuilder.HasDefaultSchema("EntityFrameworkCore.TestApplication");

            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* Eg.

            modelBuilder.Entity<Car>().HasData(
                new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
                new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
                new Car() { CarId = 3, Make = "Labourghini", Model = "Countach" });
            */
        }
    }
}