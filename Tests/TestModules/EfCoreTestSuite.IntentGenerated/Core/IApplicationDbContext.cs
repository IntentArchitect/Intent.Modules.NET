using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<A_OptionalDependent> A_OptionalDependents { get; set; }
        DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        DbSet<C_MultipleDependent> C_MultipleDependents { get; set; }
        DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        DbSet<E_RequiredDependent> E_RequiredDependents { get; set; }
        DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        DbSet<G_MultipleDependent> G_MultipleDependents { get; set; }
        DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        //DbSet<K_SelfReference> K_SelfReferences { get; set; }
        DbSet<L_SelfReferenceMultiple> L_SelfReferenceMultiples { get; set; }
        //DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}