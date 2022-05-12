using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.TPH.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.TPH.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<BaseClass> BaseClasses { get; set; }
        DbSet<DerivedClass> DerivedClasses { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}