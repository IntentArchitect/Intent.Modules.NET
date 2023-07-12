using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Application.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common.Interfaces;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.BasicAudit;
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
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence.Configurations.BasicAudit;
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
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IDomainEventService domainEventService,
            ICurrentUserService currentUserService) : base(options)
        {
            _domainEventService = domainEventService;
            _currentUserService = currentUserService;
        }

        [IntentManaged(Mode.Ignore)]
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _domainEventService = new DummyDomainEventService();
            _currentUserService = new DummyCurrentUserService();
        }

        public DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        public DbSet<TPT_AbstractBaseClass> TPT_AbstractBaseClasses { get; set; }
        public DbSet<TPH_AbstractBaseClass> TPH_AbstractBaseClasses { get; set; }
        public DbSet<TPT_AbstractBaseClassAssociated> TPT_AbstractBaseClassAssociateds { get; set; }
        public DbSet<TPH_AbstractBaseClassAssociated> TPH_AbstractBaseClassAssociateds { get; set; }
        public DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        public DbSet<ClassWithSoftDelete> ClassWithSoftDeletes { get; set; }
        public DbSet<ComplexDefaultIndex> ComplexDefaultIndices { get; set; }
        public DbSet<TPC_ConcreteBaseClass> TPC_ConcreteBaseClasses { get; set; }
        public DbSet<TPH_ConcreteBaseClass> TPH_ConcreteBaseClasses { get; set; }
        public DbSet<TPT_ConcreteBaseClass> TPT_ConcreteBaseClasses { get; set; }
        public DbSet<TPT_ConcreteBaseClassAssociated> TPT_ConcreteBaseClassAssociateds { get; set; }
        public DbSet<TPC_ConcreteBaseClassAssociated> TPC_ConcreteBaseClassAssociateds { get; set; }
        public DbSet<TPH_ConcreteBaseClassAssociated> TPH_ConcreteBaseClassAssociateds { get; set; }
        public DbSet<CustomIndex> CustomIndices { get; set; }
        public DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        public DbSet<DefaultIndex> DefaultIndices { get; set; }
        public DbSet<TPC_DerivedClassForAbstract> TPC_DerivedClassForAbstracts { get; set; }
        public DbSet<TPH_DerivedClassForAbstract> TPH_DerivedClassForAbstracts { get; set; }
        public DbSet<TPT_DerivedClassForAbstract> TPT_DerivedClassForAbstracts { get; set; }
        public DbSet<TPC_DerivedClassForAbstractAssociated> TPC_DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<TPT_DerivedClassForAbstractAssociated> TPT_DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<TPH_DerivedClassForAbstractAssociated> TPH_DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<TPH_DerivedClassForConcrete> TPH_DerivedClassForConcretes { get; set; }
        public DbSet<TPT_DerivedClassForConcrete> TPT_DerivedClassForConcretes { get; set; }
        public DbSet<TPC_DerivedClassForConcrete> TPC_DerivedClassForConcretes { get; set; }
        public DbSet<TPC_DerivedClassForConcreteAssociated> TPC_DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<TPT_DerivedClassForConcreteAssociated> TPT_DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<TPT_FkAssociatedClass> TPT_FkAssociatedClasses { get; set; }
        public DbSet<TPH_DerivedClassForConcreteAssociated> TPH_DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<DictionaryWithKvPNormal> DictionaryWithKvPNormals { get; set; }
        public DbSet<DictionaryWithKvPSerialized> DictionaryWithKvPSerializeds { get; set; }
        public DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        public DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        public DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        public DbSet<FK_A_CompositeForeignKey> FK_A_CompositeForeignKeys { get; set; }
        public DbSet<FK_B_CompositeForeignKey> FK_B_CompositeForeignKeys { get; set; }
        public DbSet<TPC_FkAssociatedClass> TPC_FkAssociatedClasses { get; set; }
        public DbSet<TPH_FkAssociatedClass> TPH_FkAssociatedClasses { get; set; }
        public DbSet<TPH_FkBaseClass> TPH_FkBaseClasses { get; set; }
        public DbSet<TPT_FkBaseClass> TPT_FkBaseClasses { get; set; }
        public DbSet<TPC_FkBaseClass> TPC_FkBaseClasses { get; set; }
        public DbSet<TPT_FkBaseClassAssociated> TPT_FkBaseClassAssociateds { get; set; }
        public DbSet<TPC_FkBaseClassAssociated> TPC_FkBaseClassAssociateds { get; set; }
        public DbSet<TPH_FkBaseClassAssociated> TPH_FkBaseClassAssociateds { get; set; }
        public DbSet<TPT_FkDerivedClass> TPT_FkDerivedClasses { get; set; }
        public DbSet<TPC_FkDerivedClass> TPC_FkDerivedClasses { get; set; }
        public DbSet<TPH_FkDerivedClass> TPH_FkDerivedClasses { get; set; }
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
        public DbSet<Audit_DerivedClass> Audit_DerivedClasses { get; set; }
        public DbSet<Audit_SoloClass> Audit_SoloClasses { get; set; }
        public DbSet<PersonWithAddressNormal> PersonWithAddressNormals { get; set; }
        public DbSet<PersonWithAddressSerialized> PersonWithAddressSerializeds { get; set; }
        public DbSet<PK_A_CompositeKey> PK_A_CompositeKeys { get; set; }
        public DbSet<PK_B_CompositeKey> PK_B_CompositeKeys { get; set; }
        public DbSet<PK_PrimaryKeyInt> PK_PrimaryKeyInts { get; set; }
        public DbSet<PK_PrimaryKeyLong> PK_PrimaryKeyLongs { get; set; }
        public DbSet<TPC_Poly_BaseClassNonAbstract> TPC_Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<TPT_Poly_BaseClassNonAbstract> TPT_Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<TPH_Poly_BaseClassNonAbstract> TPH_Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<TPT_Poly_ConcreteA> TPT_Poly_ConcreteAs { get; set; }
        public DbSet<TPC_Poly_ConcreteA> TPC_Poly_ConcreteAs { get; set; }
        public DbSet<TPH_Poly_ConcreteA> TPH_Poly_ConcreteAs { get; set; }
        public DbSet<TPC_Poly_ConcreteB> TPC_Poly_ConcreteBs { get; set; }
        public DbSet<TPH_Poly_ConcreteB> TPH_Poly_ConcreteBs { get; set; }
        public DbSet<TPT_Poly_ConcreteB> TPT_Poly_ConcreteBs { get; set; }
        public DbSet<TPT_Poly_RootAbstract> TPT_Poly_RootAbstracts { get; set; }
        public DbSet<TPH_Poly_RootAbstract> TPH_Poly_RootAbstracts { get; set; }
        public DbSet<TPC_Poly_RootAbstract_Aggr> TPC_Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<TPT_Poly_RootAbstract_Aggr> TPT_Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<TPH_Poly_RootAbstract_Aggr> TPH_Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<TPT_Poly_RootAbstract_Comp> TPT_Poly_RootAbstract_Comps { get; set; }
        public DbSet<TPC_Poly_SecondLevel> TPC_Poly_SecondLevels { get; set; }
        public DbSet<TPH_Poly_SecondLevel> TPH_Poly_SecondLevels { get; set; }
        public DbSet<TPT_Poly_SecondLevel> TPT_Poly_SecondLevels { get; set; }
        public DbSet<TPH_Poly_TopLevel> TPH_Poly_TopLevels { get; set; }
        public DbSet<TPT_Poly_TopLevel> TPT_Poly_TopLevels { get; set; }
        public DbSet<TPC_Poly_TopLevel> TPC_Poly_TopLevels { get; set; }
        public DbSet<StereotypeIndex> StereotypeIndices { get; set; }
        public DbSet<Sun> Suns { get; set; }
        public DbSet<Texture> Textures { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<Worm> Worms { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SetSoftDeleteProperties();
            SetAuditableFields();
            await DispatchEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new A_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new C_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new D_MultipleDependentConfiguration());
            modelBuilder.ApplyConfiguration(new D_OptionalAggregateConfiguration());
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
            modelBuilder.ApplyConfiguration(new N_ComplexRootConfiguration());
            modelBuilder.ApplyConfiguration(new N_CompositeOneConfiguration());
            modelBuilder.ApplyConfiguration(new N_CompositeTwoConfiguration());
            modelBuilder.ApplyConfiguration(new Audit_DerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Audit_SoloClassConfiguration());
            modelBuilder.ApplyConfiguration(new FK_A_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new FK_B_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_A_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_B_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyIntConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyLongConfiguration());
            modelBuilder.ApplyConfiguration(new ComplexDefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new CustomIndexConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new StereotypeIndexConfiguration());
            modelBuilder.ApplyConfiguration(new BranchConfiguration());
            modelBuilder.ApplyConfiguration(new InhabitantConfiguration());
            modelBuilder.ApplyConfiguration(new LeafConfiguration());
            modelBuilder.ApplyConfiguration(new SunConfiguration());
            modelBuilder.ApplyConfiguration(new TextureConfiguration());
            modelBuilder.ApplyConfiguration(new TreeConfiguration());
            modelBuilder.ApplyConfiguration(new WormConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithSoftDeleteConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new TPC_Poly_TopLevelConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_AbstractBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_AbstractBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_RootAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_Poly_TopLevelConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_AbstractBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_AbstractBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_ConcreteBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_ConcreteBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_DerivedClassForAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_DerivedClassForAbstractAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_DerivedClassForConcreteConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_DerivedClassForConcreteAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_FkAssociatedClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_FkBaseClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_FkBaseClassAssociatedConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_FkDerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_BaseClassNonAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_ConcreteAConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_ConcreteBConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_RootAbstractConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_RootAbstract_AggrConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_RootAbstract_CompConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_SecondLevelConfiguration());
            modelBuilder.ApplyConfiguration(new TPT_Poly_TopLevelConfiguration());
            modelBuilder.ApplyConfiguration(new DictionaryWithKvPNormalConfiguration());
            modelBuilder.ApplyConfiguration(new DictionaryWithKvPSerializedConfiguration());
            modelBuilder.ApplyConfiguration(new PersonWithAddressNormalConfiguration());
            modelBuilder.ApplyConfiguration(new PersonWithAddressSerializedConfiguration());
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

        private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
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
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
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

        private void SetAuditableFields()
        {
            ChangeTracker.DetectChanges();
            var userName = _currentUserService.UserName;
            var timestamp = DateTimeOffset.UtcNow;
            var entries = ChangeTracker.Entries().ToArray();

            foreach (var entry in entries)
            {
                if (entry.Entity is not IAuditable auditable)
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Modified or EntityState.Deleted:
                        auditable.UpdatedBy = userName;
                        auditable.UpdatedDate = timestamp;
                        break;
                    case EntityState.Added:
                        auditable.CreatedBy = userName;
                        auditable.CreatedDate = timestamp;
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}