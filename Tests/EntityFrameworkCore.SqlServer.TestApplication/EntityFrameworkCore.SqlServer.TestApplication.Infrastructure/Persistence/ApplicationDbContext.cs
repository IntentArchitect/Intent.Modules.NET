using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.SoftDelete;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ValueObjects;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.Associations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.NestedAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.SoftDelete;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.ValueObjects;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        [IntentManaged(Mode.Ignore)]
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _domainEventService = new DummyDomainEventService();
        }

        public DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.AbstractBaseClass> TPTInheritanceAssociationsAbstractBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.AbstractBaseClass> TPHInheritanceAssociationsAbstractBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.AbstractBaseClassAssociated> TPTInheritanceAssociationsAbstractBaseClassAssociateds { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.AbstractBaseClassAssociated> TPHInheritanceAssociationsAbstractBaseClassAssociateds { get; set; }
        public DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        public DbSet<ClassWithSoftDelete> ClassWithSoftDeletes { get; set; }
        public DbSet<ComplexDefaultIndex> ComplexDefaultIndices { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.ConcreteBaseClass> TPTInheritanceAssociationsConcreteBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.ConcreteBaseClass> TPCInheritanceAssociationsConcreteBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.ConcreteBaseClassAssociated> TPHInheritanceAssociationsConcreteBaseClassAssociateds { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.ConcreteBaseClassAssociated> TPTInheritanceAssociationsConcreteBaseClassAssociateds { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.ConcreteBaseClassAssociated> ConcreteBaseClassAssociateds { get; set; }
        public DbSet<CustomIndex> CustomIndices { get; set; }
        public DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public DbSet<DefaultIndex> DefaultIndices { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.DerivedClassForAbstract> TPHInheritanceAssociationsDerivedClassForAbstracts { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.DerivedClassForAbstract> TPTInheritanceAssociationsDerivedClassForAbstracts { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.DerivedClassForAbstractAssociated> TPCInheritanceAssociationsDerivedClassForAbstractAssociateds { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.DerivedClassForAbstractAssociated> TPTInheritanceAssociationsDerivedClassForAbstractAssociateds { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.DerivedClassForAbstractAssociated> DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.DerivedClassForConcrete> TPTInheritanceAssociationsDerivedClassForConcretes { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.DerivedClassForConcrete> TPCInheritanceAssociationsDerivedClassForConcretes { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.DerivedClassForConcreteAssociated> TPCInheritanceAssociationsDerivedClassForConcreteAssociateds { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.DerivedClassForConcreteAssociated> TPTInheritanceAssociationsDerivedClassForConcreteAssociateds { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.DerivedClassForConcreteAssociated> DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<DictionaryWithKvPNormal> DictionaryWithKvPNormals { get; set; }
        public DbSet<DictionaryWithKvPSerialized> DictionaryWithKvPSerializeds { get; set; }
        public DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public DbSet<FK_A_CompositeForeignKey> FK_A_CompositeForeignKeys { get; set; }
        public DbSet<FK_B_CompositeForeignKey> FK_B_CompositeForeignKeys { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.FkAssociatedClass> TPHInheritanceAssociationsFkAssociatedClasses { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.FkAssociatedClass> TPTInheritanceAssociationsFkAssociatedClasses { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.FkAssociatedClass> FkAssociatedClasses { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.FkBaseClass> TPHInheritanceAssociationsFkBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.FkBaseClass> TPCInheritanceAssociationsFkBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.FkBaseClass> FkBaseClasses { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.FkBaseClassAssociated> TPTInheritanceAssociationsFkBaseClassAssociateds { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.FkBaseClassAssociated> TPCInheritanceAssociationsFkBaseClassAssociateds { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.FkBaseClassAssociated> FkBaseClassAssociateds { get; set; }
        public DbSet<Domain.Entities.TPH.InheritanceAssociations.FkDerivedClass> TPHInheritanceAssociationsFkDerivedClasses { get; set; }
        public DbSet<Domain.Entities.TPC.InheritanceAssociations.FkDerivedClass> TPCInheritanceAssociationsFkDerivedClasses { get; set; }
        public DbSet<Domain.Entities.TPT.InheritanceAssociations.FkDerivedClass> FkDerivedClasses { get; set; }
        public DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        public DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        public DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        public DbSet<Inhabitant> Inhabitants { get; set; }
        public DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        public DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        public DbSet<K_SelfReference> K_SelfReferences { get; set; }
        public DbSet<L_SelfReferenceMultiple> L_SelfReferenceMultiples { get; set; }
        public DbSet<Leaf> Leaves { get; set; }
        public DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }
        public DbSet<N_ComplexRoot> N_ComplexRoots { get; set; }
        public DbSet<N_CompositeOne> N_CompositeOnes { get; set; }
        public DbSet<N_CompositeTwo> N_CompositeTwos { get; set; }
        public DbSet<PersonWithAddressNormal> PersonWithAddressNormals { get; set; }
        public DbSet<PersonWithAddressSerialized> PersonWithAddressSerializeds { get; set; }
        public DbSet<PK_A_CompositeKey> PK_A_CompositeKeys { get; set; }
        public DbSet<PK_B_CompositeKey> PK_B_CompositeKeys { get; set; }
        public DbSet<PK_PrimaryKeyInt> PK_PrimaryKeyInts { get; set; }
        public DbSet<PK_PrimaryKeyLong> PK_PrimaryKeyLongs { get; set; }
        public DbSet<Domain.Entities.TPC.Polymorphic.Poly_BaseClassNonAbstract> TPCPolymorphicPoly_BaseClassNonAbstracts { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_BaseClassNonAbstract> TPTPolymorphicPoly_BaseClassNonAbstracts { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_ConcreteA> TPTPolymorphicPoly_ConcreteAs { get; set; }
        public DbSet<Domain.Entities.TPC.Polymorphic.Poly_ConcreteA> TPCPolymorphicPoly_ConcreteAs { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_ConcreteA> Poly_ConcreteAs { get; set; }
        public DbSet<Domain.Entities.TPC.Polymorphic.Poly_ConcreteB> TPCPolymorphicPoly_ConcreteBs { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_ConcreteB> TPHPolymorphicPoly_ConcreteBs { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_ConcreteB> Poly_ConcreteBs { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_RootAbstract> TPTPolymorphicPoly_RootAbstracts { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_RootAbstract> TPHPolymorphicPoly_RootAbstracts { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_RootAbstract_Aggr> TPHPolymorphicPoly_RootAbstract_Aggrs { get; set; }
        public DbSet<Domain.Entities.TPC.Polymorphic.Poly_RootAbstract_Aggr> TPCPolymorphicPoly_RootAbstract_Aggrs { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_RootAbstract_Aggr> Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_RootAbstract_Comp> Poly_RootAbstract_Comps { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_SecondLevel> TPTPolymorphicPoly_SecondLevels { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_SecondLevel> TPHPolymorphicPoly_SecondLevels { get; set; }
        public DbSet<Domain.Entities.TPC.Polymorphic.Poly_SecondLevel> Poly_SecondLevels { get; set; }
        public DbSet<Domain.Entities.TPH.Polymorphic.Poly_TopLevel> TPHPolymorphicPoly_TopLevels { get; set; }
        public DbSet<Domain.Entities.TPT.Polymorphic.Poly_TopLevel> TPTPolymorphicPoly_TopLevels { get; set; }
        public DbSet<Domain.Entities.TPC.Polymorphic.Poly_TopLevel> Poly_TopLevels { get; set; }
        public DbSet<StereotypeIndex> StereotypeIndices { get; set; }
        public DbSet<Sun> Suns { get; set; }
        public DbSet<Texture> Textures { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<Worm> Worms { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SetSoftDeleteProperties();
            await DispatchEvents();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new A_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.AbstractBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.AbstractBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.AbstractBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.AbstractBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new BranchConfiguration());
            modelBuilder.ApplyConfiguration(new C_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithSoftDeleteConfiguration());
            modelBuilder.ApplyConfiguration(new ComplexDefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new CustomIndexConfiguration());
            modelBuilder.ApplyConfiguration(new D_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new D_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new DictionaryWithKvPNormalConfiguration());
            modelBuilder.ApplyConfiguration(new DictionaryWithKvPSerializedConfiguration());
            modelBuilder.ApplyConfiguration(new E_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new F_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new FK_A_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new FK_B_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.InheritanceAssociations.FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.InheritanceAssociations.FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.InheritanceAssociations.FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new G_RequiredCompositeNavConfiguration());
            modelBuilder.ApplyConfiguration(new H_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new H_OptionalAggregateNavConfiguration());
            modelBuilder.ApplyConfiguration(new InhabitantConfiguration());
            modelBuilder.ApplyConfiguration(new J_MultipleAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new J_RequiredDependentConfiguration());
            modelBuilder.ApplyConfiguration(new K_SelfReferenceConfiguration());
            modelBuilder.ApplyConfiguration(new L_SelfReferenceMultipleConfiguration());
            modelBuilder.ApplyConfiguration(new LeafConfiguration());
            modelBuilder.ApplyConfiguration(new M_SelfReferenceBiNavConfiguration());
            modelBuilder.ApplyConfiguration(new N_ComplexRootConfiguration());
            modelBuilder.ApplyConfiguration(new N_CompositeOneConfiguration());
            modelBuilder.ApplyConfiguration(new N_CompositeTwoConfiguration());
            modelBuilder.ApplyConfiguration(new PersonWithAddressNormalConfiguration());
            modelBuilder.ApplyConfiguration(new PersonWithAddressSerializedConfiguration());
            modelBuilder.ApplyConfiguration(new PK_A_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_B_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyIntConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyLongConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.Polymorphic.Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.Polymorphic.Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.Polymorphic.Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_RootAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_RootAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.Polymorphic.Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new Poly_RootAbstract_CompConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.Polymorphic.Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPH.Polymorphic.Poly_TopLevelConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPT.Polymorphic.Poly_TopLevelConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TPC.Polymorphic.Poly_TopLevelConfiguration());
            modelBuilder.ApplyConfiguration(new StereotypeIndexConfiguration());
            modelBuilder.ApplyConfiguration(new SunConfiguration());
            modelBuilder.ApplyConfiguration(new TextureConfiguration());
            modelBuilder.ApplyConfiguration(new TreeConfiguration());
            modelBuilder.ApplyConfiguration(new WormConfiguration());
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

        private void SetSoftDeleteProperties()
        {
            ChangeTracker.DetectChanges();

            var entities = ChangeTracker
                .Entries()
                .Where(t => t.Entity is ISoftDelete && t.State == EntityState.Deleted)
                .ToArray();

            if (!entities.Any())
            {
                return;
            }

            foreach (var entry in entities)
            {
                var entity = (ISoftDelete)entry.Entity;
                entity.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
        }
    }
}