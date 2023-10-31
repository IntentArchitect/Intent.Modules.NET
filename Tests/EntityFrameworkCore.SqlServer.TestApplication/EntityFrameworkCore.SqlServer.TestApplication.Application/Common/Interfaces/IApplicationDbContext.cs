using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Accounts;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Accounts.NotSchema;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.BasicAudit;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NotSchema;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.SoftDelete;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ValueObjects;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<SchemaParent> SchemaParents { get; }
        DbSet<Table> Tables { get; }
        DbSet<TableExplicitSchema> TableExplicitSchemas { get; }
        DbSet<TableOverride> TableOverrides { get; }
        DbSet<TablePlain> TablePlains { get; }
        DbSet<View> Views { get; }
        DbSet<ViewExplicitSchema> ViewExplicitSchemas { get; }
        DbSet<ViewOverride> ViewOverrides { get; }
        DbSet<AccTable> AccTables { get; }
        DbSet<AccTableOverride> AccTableOverrides { get; }
        DbSet<AccView> AccViews { get; }
        DbSet<AccViewOverride> AccViewOverrides { get; }
        DbSet<AccTableFolder> AccTableFolders { get; }
        DbSet<AccViewFolder> AccViewFolders { get; }
        DbSet<A_RequiredComposite> A_RequiredComposites { get; }
        DbSet<Author> Authors { get; }
        DbSet<B_OptionalAggregate> B_OptionalAggregates { get; }
        DbSet<B_OptionalDependent> B_OptionalDependents { get; }
        DbSet<Book> Books { get; }
        DbSet<C_RequiredComposite> C_RequiredComposites { get; }
        DbSet<D_MultipleDependent> D_MultipleDependents { get; }
        DbSet<D_OptionalAggregate> D_OptionalAggregates { get; }
        DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; }
        DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; }
        DbSet<F_OptionalDependent> F_OptionalDependents { get; }
        DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; }
        DbSet<H_MultipleDependent> H_MultipleDependents { get; }
        DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; }
        DbSet<J_MultipleAggregate> J_MultipleAggregates { get; }
        DbSet<J_RequiredDependent> J_RequiredDependents { get; }
        DbSet<K_SelfReference> K_SelfReferences { get; }
        DbSet<L_SelfReferenceMultiple> L_SelfReferenceMultiples { get; }
        DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; }
        DbSet<N_ComplexRoot> N_ComplexRoots { get; }
        DbSet<N_CompositeOne> N_CompositeOnes { get; }
        DbSet<N_CompositeTwo> N_CompositeTwos { get; }
        DbSet<O_DestNameDiff> O_DestNameDiffs { get; }
        DbSet<P_SourceNameDiff> P_SourceNameDiffs { get; }
        DbSet<Q_DestNameDiff> Q_DestNameDiffs { get; }
        DbSet<R_SourceNameDiff> R_SourceNameDiffs { get; }
        DbSet<Audit_DerivedClass> Audit_DerivedClasses { get; }
        DbSet<Audit_SoloClass> Audit_SoloClasses { get; }
        DbSet<ChildNonStdId> ChildNonStdIds { get; }
        DbSet<FK_A_CompositeForeignKey> FK_A_CompositeForeignKeys { get; }
        DbSet<FK_B_CompositeForeignKey> FK_B_CompositeForeignKeys { get; }
        DbSet<ParentNonStdId> ParentNonStdIds { get; }
        DbSet<PK_A_CompositeKey> PK_A_CompositeKeys { get; }
        DbSet<PK_B_CompositeKey> PK_B_CompositeKeys { get; }
        DbSet<PK_PrimaryKeyInt> PK_PrimaryKeyInts { get; }
        DbSet<PK_PrimaryKeyLong> PK_PrimaryKeyLongs { get; }
        DbSet<ComplexDefaultIndex> ComplexDefaultIndices { get; }
        DbSet<CustomIndex> CustomIndices { get; }
        DbSet<DefaultIndex> DefaultIndices { get; }
        DbSet<ParentIndex> ParentIndices { get; }
        DbSet<SortDirectionIndex> SortDirectionIndices { get; }
        DbSet<SortDirectionStereotype> SortDirectionStereotypes { get; }
        DbSet<StereotypeIndex> StereotypeIndices { get; }
        DbSet<Branch> Branches { get; }
        DbSet<Inhabitant> Inhabitants { get; }
        DbSet<Leaf> Leaves { get; }
        DbSet<Sun> Suns { get; }
        DbSet<Texture> Textures { get; }
        DbSet<Tree> Trees { get; }
        DbSet<Worm> Worms { get; }
        DbSet<TableFolder> TableFolders { get; }
        DbSet<ViewFolder> ViewFolders { get; }
        DbSet<ClassWithSoftDelete> ClassWithSoftDeletes { get; }
        DbSet<TPC_ConcreteBaseClass> TPC_ConcreteBaseClasses { get; }
        DbSet<TPC_ConcreteBaseClassAssociated> TPC_ConcreteBaseClassAssociateds { get; }
        DbSet<TPC_DerivedClassForAbstract> TPC_DerivedClassForAbstracts { get; }
        DbSet<TPC_DerivedClassForAbstractAssociated> TPC_DerivedClassForAbstractAssociateds { get; }
        DbSet<TPC_DerivedClassForConcrete> TPC_DerivedClassForConcretes { get; }
        DbSet<TPC_DerivedClassForConcreteAssociated> TPC_DerivedClassForConcreteAssociateds { get; }
        DbSet<TPC_FkAssociatedClass> TPC_FkAssociatedClasses { get; }
        DbSet<TPC_FkBaseClass> TPC_FkBaseClasses { get; }
        DbSet<TPC_FkBaseClassAssociated> TPC_FkBaseClassAssociateds { get; }
        DbSet<TPC_FkDerivedClass> TPC_FkDerivedClasses { get; }
        DbSet<TPC_Poly_BaseClassNonAbstract> TPC_Poly_BaseClassNonAbstracts { get; }
        DbSet<TPC_Poly_ConcreteA> TPC_Poly_ConcreteAs { get; }
        DbSet<TPC_Poly_ConcreteB> TPC_Poly_ConcreteBs { get; }
        DbSet<TPC_Poly_RootAbstract_Aggr> TPC_Poly_RootAbstract_Aggrs { get; }
        DbSet<TPC_Poly_SecondLevel> TPC_Poly_SecondLevels { get; }
        DbSet<TPC_Poly_TopLevel> TPC_Poly_TopLevels { get; }
        DbSet<TPH_AbstractBaseClass> TPH_AbstractBaseClasses { get; }
        DbSet<TPH_AbstractBaseClassAssociated> TPH_AbstractBaseClassAssociateds { get; }
        DbSet<TPH_ConcreteBaseClass> TPH_ConcreteBaseClasses { get; }
        DbSet<TPH_ConcreteBaseClassAssociated> TPH_ConcreteBaseClassAssociateds { get; }
        DbSet<TPH_DerivedClassForAbstract> TPH_DerivedClassForAbstracts { get; }
        DbSet<TPH_DerivedClassForAbstractAssociated> TPH_DerivedClassForAbstractAssociateds { get; }
        DbSet<TPH_DerivedClassForConcrete> TPH_DerivedClassForConcretes { get; }
        DbSet<TPH_DerivedClassForConcreteAssociated> TPH_DerivedClassForConcreteAssociateds { get; }
        DbSet<TPH_FkAssociatedClass> TPH_FkAssociatedClasses { get; }
        DbSet<TPH_FkBaseClass> TPH_FkBaseClasses { get; }
        DbSet<TPH_FkBaseClassAssociated> TPH_FkBaseClassAssociateds { get; }
        DbSet<TPH_FkDerivedClass> TPH_FkDerivedClasses { get; }
        DbSet<TPH_MiddleAbstract_Leaf> TPH_MiddleAbstract_Leaves { get; }
        DbSet<TPH_MiddleAbstract_Root> TPH_MiddleAbstract_Roots { get; }
        DbSet<TPH_Poly_BaseClassNonAbstract> TPH_Poly_BaseClassNonAbstracts { get; }
        DbSet<TPH_Poly_ConcreteA> TPH_Poly_ConcreteAs { get; }
        DbSet<TPH_Poly_ConcreteB> TPH_Poly_ConcreteBs { get; }
        DbSet<TPH_Poly_RootAbstract> TPH_Poly_RootAbstracts { get; }
        DbSet<TPH_Poly_RootAbstract_Aggr> TPH_Poly_RootAbstract_Aggrs { get; }
        DbSet<TPH_Poly_SecondLevel> TPH_Poly_SecondLevels { get; }
        DbSet<TPH_Poly_TopLevel> TPH_Poly_TopLevels { get; }
        DbSet<TPT_AbstractBaseClass> TPT_AbstractBaseClasses { get; }
        DbSet<TPT_AbstractBaseClassAssociated> TPT_AbstractBaseClassAssociateds { get; }
        DbSet<TPT_ConcreteBaseClass> TPT_ConcreteBaseClasses { get; }
        DbSet<TPT_ConcreteBaseClassAssociated> TPT_ConcreteBaseClassAssociateds { get; }
        DbSet<TPT_DerivedClassForAbstract> TPT_DerivedClassForAbstracts { get; }
        DbSet<TPT_DerivedClassForAbstractAssociated> TPT_DerivedClassForAbstractAssociateds { get; }
        DbSet<TPT_DerivedClassForConcrete> TPT_DerivedClassForConcretes { get; }
        DbSet<TPT_DerivedClassForConcreteAssociated> TPT_DerivedClassForConcreteAssociateds { get; }
        DbSet<TPT_FkAssociatedClass> TPT_FkAssociatedClasses { get; }
        DbSet<TPT_FkBaseClass> TPT_FkBaseClasses { get; }
        DbSet<TPT_FkBaseClassAssociated> TPT_FkBaseClassAssociateds { get; }
        DbSet<TPT_FkDerivedClass> TPT_FkDerivedClasses { get; }
        DbSet<TPT_Poly_BaseClassNonAbstract> TPT_Poly_BaseClassNonAbstracts { get; }
        DbSet<TPT_Poly_ConcreteA> TPT_Poly_ConcreteAs { get; }
        DbSet<TPT_Poly_ConcreteB> TPT_Poly_ConcreteBs { get; }
        DbSet<TPT_Poly_RootAbstract> TPT_Poly_RootAbstracts { get; }
        DbSet<TPT_Poly_RootAbstract_Aggr> TPT_Poly_RootAbstract_Aggrs { get; }
        DbSet<TPT_Poly_RootAbstract_Comp> TPT_Poly_RootAbstract_Comps { get; }
        DbSet<TPT_Poly_SecondLevel> TPT_Poly_SecondLevels { get; }
        DbSet<TPT_Poly_TopLevel> TPT_Poly_TopLevels { get; }
        DbSet<DictionaryWithKvPNormal> DictionaryWithKvPNormals { get; }
        DbSet<DictionaryWithKvPSerialized> DictionaryWithKvPSerializeds { get; }
        DbSet<PersonWithAddressNormal> PersonWithAddressNormals { get; }
        DbSet<PersonWithAddressSerialized> PersonWithAddressSerializeds { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}