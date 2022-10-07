using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.NestedComposition;
using EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<A_RequiredComposite> A_RequiredComposites { get; set; }
        DbSet<AbstractBaseClass> AbstractBaseClasses { get; set; }
        DbSet<AbstractBaseClassAssociated> AbstractBaseClassAssociateds { get; set; }
        DbSet<B_OptionalAggregate> B_OptionalAggregates { get; set; }
        DbSet<B_OptionalDependent> B_OptionalDependents { get; set; }
        DbSet<C_RequiredComposite> C_RequiredComposites { get; set; }
        DbSet<ClassA> ClassAs { get; set; }
        DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        DbSet<ConcreteBaseClassAssociated> ConcreteBaseClassAssociateds { get; set; }
        DbSet<D_MultipleDependent> D_MultipleDependents { get; set; }
        DbSet<D_OptionalAggregate> D_OptionalAggregates { get; set; }
        DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        DbSet<DerivedClassForAbstractAssociated> DerivedClassForAbstractAssociateds { get; set; }
        DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        DbSet<DerivedClassForConcreteAssociated> DerivedClassForConcreteAssociateds { get; set; }
        DbSet<E_RequiredCompositeNav> E_RequiredCompositeNavs { get; set; }
        DbSet<F_OptionalAggregateNav> F_OptionalAggregateNavs { get; set; }
        DbSet<F_OptionalDependent> F_OptionalDependents { get; set; }
        DbSet<G_RequiredCompositeNav> G_RequiredCompositeNavs { get; set; }
        DbSet<H_MultipleDependent> H_MultipleDependents { get; set; }
        DbSet<H_OptionalAggregateNav> H_OptionalAggregateNavs { get; set; }
        DbSet<J_MultipleAggregate> J_MultipleAggregates { get; set; }
        DbSet<J_RequiredDependent> J_RequiredDependents { get; set; }
        DbSet<K_SelfReference> K_SelfReferences { get; set; }
        DbSet<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; }
        DbSet<Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts { get; set; }
        DbSet<Poly_ConcreteA> Poly_ConcreteAs { get; set; }
        DbSet<Poly_ConcreteB> Poly_ConcreteBs { get; set; }
        DbSet<Poly_RootAbstract> Poly_RootAbstracts { get; set; }
        DbSet<Poly_RootAbstract_Aggr> Poly_RootAbstract_Aggrs { get; set; }
        DbSet<Poly_SecondLevel> Poly_SecondLevels { get; set; }
        DbSet<Poly_TopLevel> Poly_TopLevels { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}