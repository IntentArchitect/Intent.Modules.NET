using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Application.Common.Interfaces;
using EntityFrameworkCore.Postgres.Domain.Common.Interfaces;
using EntityFrameworkCore.Postgres.Domain.Entities;
using EntityFrameworkCore.Postgres.Domain.Entities.Accounts;
using EntityFrameworkCore.Postgres.Domain.Entities.Accounts.NotSchema;
using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using EntityFrameworkCore.Postgres.Domain.Entities.BasicAudit;
using EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.Postgres.Domain.Entities.Geometry;
using EntityFrameworkCore.Postgres.Domain.Entities.Indexes;
using EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations;
using EntityFrameworkCore.Postgres.Domain.Entities.NotSchema;
using EntityFrameworkCore.Postgres.Domain.Entities.SoftDelete;
using EntityFrameworkCore.Postgres.Domain.Entities.TimeConcepts;
using EntityFrameworkCore.Postgres.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Domain.Entities.TPC.Polymorphic;
using EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Domain.Entities.TPH.Polymorphic;
using EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Domain.Entities.TPT.Polymorphic;
using EntityFrameworkCore.Postgres.Domain.Entities.ValueObjects;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Accounts;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Accounts.NotSchema;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Associations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.BasicAudit;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Converters;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.ExplicitKeys;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Geometry;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.Indexes;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.NestedAssociations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.NotSchema;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.SoftDelete;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TimeConcepts;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPC.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPC.Polymorphic;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPH.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPH.Polymorphic;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPT.InheritanceAssociations;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.TPT.Polymorphic;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence.Configurations.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<SchemaParent> SchemaParents { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<TableExplicitSchema> TableExplicitSchemas { get; set; }
        public DbSet<TableOverride> TableOverrides { get; set; }
        public DbSet<TablePlain> TablePlains { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<ViewExplicitSchema> ViewExplicitSchemas { get; set; }
        public DbSet<ViewOverride> ViewOverrides { get; set; }
        public DbSet<AccTable> AccTables { get; set; }
        public DbSet<AccTableOverride> AccTableOverrides { get; set; }
        public DbSet<AccView> AccViews { get; set; }
        public DbSet<AccViewOverride> AccViewOverrides { get; set; }
        public DbSet<AccTableFolder> AccTableFolders { get; set; }
        public DbSet<AccViewFolder> AccViewFolders { get; set; }
        public DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        public DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        public DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        public DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
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
        public DbSet<N_ComplexRoot> N_ComplexRoots { get; set; }
        public DbSet<N_CompositeOne> N_CompositeOnes { get; set; }
        public DbSet<N_CompositeTwo> N_CompositeTwos { get; set; }
        public DbSet<O_DestNameDiff> O_DestNameDiffs { get; set; }
        public DbSet<P_SourceNameDiff> P_SourceNameDiffs { get; set; }
        public DbSet<Q_DestNameDiff> Q_DestNameDiffs { get; set; }
        public DbSet<R_SourceNameDiff> R_SourceNameDiffs { get; set; }
        public DbSet<Audit_DerivedClass> Audit_DerivedClasses { get; set; }
        public DbSet<Audit_SoloClass> Audit_SoloClasses { get; set; }
        public DbSet<ChildNonStdId> ChildNonStdIds { get; set; }
        public DbSet<FK_A_CompositeForeignKey> FK_A_CompositeForeignKeys { get; set; }
        public DbSet<FK_B_CompositeForeignKey> FK_B_CompositeForeignKeys { get; set; }
        public DbSet<ParentNonStdId> ParentNonStdIds { get; set; }
        public DbSet<PK_A_CompositeKey> PK_A_CompositeKeys { get; set; }
        public DbSet<PK_B_CompositeKey> PK_B_CompositeKeys { get; set; }
        public DbSet<PK_PrimaryKeyInt> PK_PrimaryKeyInts { get; set; }
        public DbSet<PK_PrimaryKeyLong> PK_PrimaryKeyLongs { get; set; }
        public DbSet<GeometryType> GeometryTypes { get; set; }
        public DbSet<ComplexDefaultIndex> ComplexDefaultIndices { get; set; }
        public DbSet<CustomIndex> CustomIndices { get; set; }
        public DbSet<DefaultIndex> DefaultIndices { get; set; }
        public DbSet<DeviationIndex> DeviationIndices { get; set; }
        public DbSet<ParentIndex> ParentIndices { get; set; }
        public DbSet<SortDirectionIndex> SortDirectionIndices { get; set; }
        public DbSet<SortDirectionStereotype> SortDirectionStereotypes { get; set; }
        public DbSet<StereotypeIndex> StereotypeIndices { get; set; }
        public DbSet<WithBaseIndex> WithBaseIndices { get; set; }
        public DbSet<WithBaseIndexBase> WithBaseIndexBases { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Inhabitant> Inhabitants { get; set; }
        public DbSet<Leaf> Leaves { get; set; }
        public DbSet<Sun> Suns { get; set; }
        public DbSet<Texture> Textures { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<Worm> Worms { get; set; }
        public DbSet<TableFolder> TableFolders { get; set; }
        public DbSet<ViewFolder> ViewFolders { get; set; }
        public DbSet<ClassWithSoftDelete> ClassWithSoftDeletes { get; set; }
        public DbSet<TimeEntity> TimeEntities { get; set; }
        public DbSet<TPC_ConcreteBaseClass> TPC_ConcreteBaseClasses { get; set; }
        public DbSet<TPC_ConcreteBaseClassAssociated> TPC_ConcreteBaseClassAssociateds { get; set; }
        public DbSet<TPC_DerivedClassForAbstract> TPC_DerivedClassForAbstracts { get; set; }
        public DbSet<TPC_DerivedClassForAbstractAssociated> TPC_DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<TPC_DerivedClassForConcrete> TPC_DerivedClassForConcretes { get; set; }
        public DbSet<TPC_DerivedClassForConcreteAssociated> TPC_DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<TPC_FkAssociatedClass> TPC_FkAssociatedClasses { get; set; }
        public DbSet<TPC_FkBaseClass> TPC_FkBaseClasses { get; set; }
        public DbSet<TPC_FkBaseClassAssociated> TPC_FkBaseClassAssociateds { get; set; }
        public DbSet<TPC_FkDerivedClass> TPC_FkDerivedClasses { get; set; }
        public DbSet<TPC_Poly_BaseClassNonAbstract> TPC_Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<TPC_Poly_ConcreteA> TPC_Poly_ConcreteAs { get; set; }
        public DbSet<TPC_Poly_ConcreteB> TPC_Poly_ConcreteBs { get; set; }
        public DbSet<TPC_Poly_RootAbstract_Aggr> TPC_Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<TPC_Poly_SecondLevel> TPC_Poly_SecondLevels { get; set; }
        public DbSet<TPC_Poly_TopLevel> TPC_Poly_TopLevels { get; set; }
        public DbSet<TPH_AbstractBaseClass> TPH_AbstractBaseClasses { get; set; }
        public DbSet<TPH_AbstractBaseClassAssociated> TPH_AbstractBaseClassAssociateds { get; set; }
        public DbSet<TPH_ConcreteBaseClass> TPH_ConcreteBaseClasses { get; set; }
        public DbSet<TPH_ConcreteBaseClassAssociated> TPH_ConcreteBaseClassAssociateds { get; set; }
        public DbSet<TPH_DerivedClassForAbstract> TPH_DerivedClassForAbstracts { get; set; }
        public DbSet<TPH_DerivedClassForAbstractAssociated> TPH_DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<TPH_DerivedClassForConcrete> TPH_DerivedClassForConcretes { get; set; }
        public DbSet<TPH_DerivedClassForConcreteAssociated> TPH_DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<TPH_FkAssociatedClass> TPH_FkAssociatedClasses { get; set; }
        public DbSet<TPH_FkBaseClass> TPH_FkBaseClasses { get; set; }
        public DbSet<TPH_FkBaseClassAssociated> TPH_FkBaseClassAssociateds { get; set; }
        public DbSet<TPH_FkDerivedClass> TPH_FkDerivedClasses { get; set; }
        public DbSet<TPH_MiddleAbstract_Leaf> TPH_MiddleAbstract_Leaves { get; set; }
        public DbSet<TPH_MiddleAbstract_Root> TPH_MiddleAbstract_Roots { get; set; }
        public DbSet<TPH_Poly_BaseClassNonAbstract> TPH_Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<TPH_Poly_ConcreteA> TPH_Poly_ConcreteAs { get; set; }
        public DbSet<TPH_Poly_ConcreteB> TPH_Poly_ConcreteBs { get; set; }
        public DbSet<TPH_Poly_RootAbstract> TPH_Poly_RootAbstracts { get; set; }
        public DbSet<TPH_Poly_RootAbstract_Aggr> TPH_Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<TPH_Poly_SecondLevel> TPH_Poly_SecondLevels { get; set; }
        public DbSet<TPH_Poly_TopLevel> TPH_Poly_TopLevels { get; set; }
        public DbSet<TPT_AbstractBaseClass> TPT_AbstractBaseClasses { get; set; }
        public DbSet<TPT_AbstractBaseClassAssociated> TPT_AbstractBaseClassAssociateds { get; set; }
        public DbSet<TPT_ConcreteBaseClass> TPT_ConcreteBaseClasses { get; set; }
        public DbSet<TPT_ConcreteBaseClassAssociated> TPT_ConcreteBaseClassAssociateds { get; set; }
        public DbSet<TPT_DerivedClassForAbstract> TPT_DerivedClassForAbstracts { get; set; }
        public DbSet<TPT_DerivedClassForAbstractAssociated> TPT_DerivedClassForAbstractAssociateds { get; set; }
        public DbSet<TPT_DerivedClassForConcrete> TPT_DerivedClassForConcretes { get; set; }
        public DbSet<TPT_DerivedClassForConcreteAssociated> TPT_DerivedClassForConcreteAssociateds { get; set; }
        public DbSet<TPT_FkAssociatedClass> TPT_FkAssociatedClasses { get; set; }
        public DbSet<TPT_FkBaseClass> TPT_FkBaseClasses { get; set; }
        public DbSet<TPT_FkBaseClassAssociated> TPT_FkBaseClassAssociateds { get; set; }
        public DbSet<TPT_FkDerivedClass> TPT_FkDerivedClasses { get; set; }
        public DbSet<TPT_Poly_BaseClassNonAbstract> TPT_Poly_BaseClassNonAbstracts { get; set; }
        public DbSet<TPT_Poly_ConcreteA> TPT_Poly_ConcreteAs { get; set; }
        public DbSet<TPT_Poly_ConcreteB> TPT_Poly_ConcreteBs { get; set; }
        public DbSet<TPT_Poly_RootAbstract> TPT_Poly_RootAbstracts { get; set; }
        public DbSet<TPT_Poly_RootAbstract_Aggr> TPT_Poly_RootAbstract_Aggrs { get; set; }
        public DbSet<TPT_Poly_RootAbstract_Comp> TPT_Poly_RootAbstract_Comps { get; set; }
        public DbSet<TPT_Poly_SecondLevel> TPT_Poly_SecondLevels { get; set; }
        public DbSet<TPT_Poly_TopLevel> TPT_Poly_TopLevels { get; set; }
        public DbSet<DictionaryWithKvPNormal> DictionaryWithKvPNormals { get; set; }
        public DbSet<DictionaryWithKvPSerialized> DictionaryWithKvPSerializeds { get; set; }
        public DbSet<PersonWithAddressNormal> PersonWithAddressNormals { get; set; }
        public DbSet<PersonWithAddressSerialized> PersonWithAddressSerializeds { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetAuditableFields();
            SetSoftDeleteProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SetAuditableFields();
            SetSoftDeleteProperties();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new SchemaParentConfiguration());
            modelBuilder.ApplyConfiguration(new TableConfiguration());
            modelBuilder.ApplyConfiguration(new TableExplicitSchemaConfiguration());
            modelBuilder.ApplyConfiguration(new TableOverrideConfiguration());
            modelBuilder.ApplyConfiguration(new TablePlainConfiguration());
            modelBuilder.ApplyConfiguration(new ViewConfiguration());
            modelBuilder.ApplyConfiguration(new ViewExplicitSchemaConfiguration());
            modelBuilder.ApplyConfiguration(new ViewOverrideConfiguration());
            modelBuilder.ApplyConfiguration(new AccTableConfiguration());
            modelBuilder.ApplyConfiguration(new AccTableOverrideConfiguration());
            modelBuilder.ApplyConfiguration(new AccViewConfiguration());
            modelBuilder.ApplyConfiguration(new AccViewOverrideConfiguration());
            modelBuilder.ApplyConfiguration(new AccTableFolderConfiguration());
            modelBuilder.ApplyConfiguration(new AccViewFolderConfiguration());
            modelBuilder.ApplyConfiguration(new A_RequiredCompositeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalAggregateConfiguration());
            modelBuilder.ApplyConfiguration(new B_OptionalDependentConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
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
            modelBuilder.ApplyConfiguration(new O_DestNameDiffConfiguration());
            modelBuilder.ApplyConfiguration(new P_SourceNameDiffConfiguration());
            modelBuilder.ApplyConfiguration(new Q_DestNameDiffConfiguration());
            modelBuilder.ApplyConfiguration(new R_SourceNameDiffConfiguration());
            modelBuilder.ApplyConfiguration(new Audit_DerivedClassConfiguration());
            modelBuilder.ApplyConfiguration(new Audit_SoloClassConfiguration());
            modelBuilder.ApplyConfiguration(new ChildNonStdIdConfiguration());
            modelBuilder.ApplyConfiguration(new FK_A_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new FK_B_CompositeForeignKeyConfiguration());
            modelBuilder.ApplyConfiguration(new ParentNonStdIdConfiguration());
            modelBuilder.ApplyConfiguration(new PK_A_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_B_CompositeKeyConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyIntConfiguration());
            modelBuilder.ApplyConfiguration(new PK_PrimaryKeyLongConfiguration());
            modelBuilder.ApplyConfiguration(new GeometryTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ComplexDefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new CustomIndexConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultIndexConfiguration());
            modelBuilder.ApplyConfiguration(new DeviationIndexConfiguration());
            modelBuilder.ApplyConfiguration(new ParentIndexConfiguration());
            modelBuilder.ApplyConfiguration(new SortDirectionIndexConfiguration());
            modelBuilder.ApplyConfiguration(new SortDirectionStereotypeConfiguration());
            modelBuilder.ApplyConfiguration(new StereotypeIndexConfiguration());
            modelBuilder.ApplyConfiguration(new WithBaseIndexConfiguration());
            modelBuilder.ApplyConfiguration(new WithBaseIndexBaseConfiguration());
            modelBuilder.ApplyConfiguration(new BranchConfiguration());
            modelBuilder.ApplyConfiguration(new InhabitantConfiguration());
            modelBuilder.ApplyConfiguration(new LeafConfiguration());
            modelBuilder.ApplyConfiguration(new SunConfiguration());
            modelBuilder.ApplyConfiguration(new TextureConfiguration());
            modelBuilder.ApplyConfiguration(new TreeConfiguration());
            modelBuilder.ApplyConfiguration(new WormConfiguration());
            modelBuilder.ApplyConfiguration(new TableFolderConfiguration());
            modelBuilder.ApplyConfiguration(new ViewFolderConfiguration());
            modelBuilder.ApplyConfiguration(new ClassWithSoftDeleteConfiguration());
            modelBuilder.ApplyConfiguration(new TimeEntityConfiguration());
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
            modelBuilder.ApplyConfiguration(new TPH_MiddleAbstract_LeafConfiguration());
            modelBuilder.ApplyConfiguration(new TPH_MiddleAbstract_RootConfiguration());
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
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties(typeof(DateTimeOffset)).HaveConversion(typeof(UtcDateTimeOffsetConverter));
        }

        private void SetAuditableFields()
        {
            var auditableEntries = ChangeTracker.Entries()
                .Where(entry => entry.State is EntityState.Added or EntityState.Deleted or EntityState.Modified &&
                                entry.Entity is IAuditable)
                .Select(entry => new
                {
                    entry.State,
                    Property = new Func<string, PropertyEntry>(entry.Property),
                    Auditable = (IAuditable)entry.Entity
                })
                .ToArray();

            if (!auditableEntries.Any())
            {
                return;
            }

            var userIdentifier = _currentUserService.UserId ?? throw new InvalidOperationException("UserId is null");
            var timestamp = DateTimeOffset.UtcNow;

            foreach (var entry in auditableEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Auditable.SetCreated(userIdentifier, timestamp);
                        break;
                    case EntityState.Modified or EntityState.Deleted:
                        entry.Auditable.SetUpdated(userIdentifier, timestamp);
                        entry.Property("CreatedBy").IsModified = false;
                        entry.Property("CreatedDate").IsModified = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SetSoftDeleteProperties()
        {
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