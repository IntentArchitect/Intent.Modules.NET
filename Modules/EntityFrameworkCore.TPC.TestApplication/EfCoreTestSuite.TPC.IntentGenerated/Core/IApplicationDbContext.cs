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
        DbSet<ConcreteBaseClass> ConcreteBaseClasses { get; set; }
        DbSet<DerivedClassForAbstract> DerivedClassForAbstracts { get; set; }
        DbSet<DerivedClassForConcrete> DerivedClassForConcretes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}