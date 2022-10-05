using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.TPC.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPC.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<A_OwnerClass> A_OwnerClasses { get; set; }
        DbSet<A_WeirdClass> A_WeirdClasses { get; set; }
        DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        DbSet<ConcreteBaseClassAssociated> ConcreteBaseClassAssociateds { get; set; }
        DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        DbSet<DerivedClassForAbstractAssociated> DerivedClassForAbstractAssociateds { get; set; }
        DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        DbSet<DerivedClassForConcreteAssociated> DerivedClassForConcreteAssociateds { get; set; }
        DbSet<FkAssociatedClass> FkAssociatedClasses { get; set; }
        DbSet<FkBaseClass> FkBaseClasses { get; set; }
        DbSet<FkBaseClassAssociated> FkBaseClassAssociateds { get; set; }
        DbSet<FkDerivedClass> FkDerivedClasses { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}