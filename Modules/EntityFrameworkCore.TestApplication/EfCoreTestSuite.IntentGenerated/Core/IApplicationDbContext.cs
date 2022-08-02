using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.IntentGenerated.Entities;
using EfCoreTestSuite.IntentGenerated.Entities.Associations;
using EfCoreTestSuite.IntentGenerated.Entities.ExplicitKeys;
using EfCoreTestSuite.IntentGenerated.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        DbSet<ComplexDefaultIndex> ComplexDefaultIndexes { get; set; }
        DbSet<CustomIndex> CustomIndexes { get; set; }
        DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        DbSet<DefaultIndex> DefaultIndexes { get; set; }
        DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        DbSet<E2_RequiredCompositeNav> E2_RequiredCompositeNavs { get; set; }
        DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        DbSet<FK_A_CompositeForeignKey> FK_A_CompositeForeignKeys { get; set; }
        DbSet<FK_B_CompositeForeignKey> FK_B_CompositeForeignKeys { get; set; }
        DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        DbSet<K_SelfReference> K_SelfReferences { get; set; }
        DbSet<L_SelfReferenceMultiple> L_SelfReferenceMultiples { get; set; }
        DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }
        DbSet<PK_A_CompositeKey> PK_A_CompositeKeys { get; set; }
        DbSet<PK_B_CompositeKey> PK_B_CompositeKeys { get; set; }
        DbSet<PK_PrimaryKeyInt> PK_PrimaryKeyInts { get; set; }
        DbSet<PK_PrimaryKeyLong> PK_PrimaryKeyLongs { get; set; }
        DbSet<StereotypeIndex> StereotypeIndexes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}