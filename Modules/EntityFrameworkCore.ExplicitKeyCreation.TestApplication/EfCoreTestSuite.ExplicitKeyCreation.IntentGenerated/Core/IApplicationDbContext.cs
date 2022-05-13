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
        DbSet<FK_ExplicitKeys_CompositeForeignKey> FK_ExplicitKeys_CompositeForeignKeys { get; set; }
        DbSet<PK_ExplicitKeys_CompositeKey> PK_ExplicitKeys_CompositeKeys { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}