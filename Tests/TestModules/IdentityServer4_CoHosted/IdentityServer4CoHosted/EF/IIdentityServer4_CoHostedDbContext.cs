using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace IdentityServer4CoHosted.EF
{
    public interface IIdentityServer4_CoHostedDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}