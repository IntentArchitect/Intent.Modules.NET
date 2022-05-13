using System.Threading;
using System.Threading.Tasks;
using EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace EfCoreTestSuite.ExplicitKeyCreation.IntentGenerated.Core
{
    public interface IApplicationDbContext
    {
        DbSet<ExplicitKeysCompositeForeignKey> ExplicitKeysCompositeForeignKeys { get; set; }
        DbSet<ExplicitKeysCompositeKey> ExplicitKeysCompositeKeys { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}