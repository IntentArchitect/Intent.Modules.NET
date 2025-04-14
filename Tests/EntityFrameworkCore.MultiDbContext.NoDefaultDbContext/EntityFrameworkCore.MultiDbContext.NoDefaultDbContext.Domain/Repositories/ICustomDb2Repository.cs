using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories
{
    public interface ICustomDb2Repository
    {
        Task TestProc(CancellationToken cancellationToken = default);
    }
}