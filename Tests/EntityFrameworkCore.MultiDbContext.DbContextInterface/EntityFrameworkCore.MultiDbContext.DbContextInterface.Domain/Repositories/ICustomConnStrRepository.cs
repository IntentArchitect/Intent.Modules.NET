using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Repositories
{
    public interface ICustomConnStrRepository
    {
        Task TestProc(CancellationToken cancellationToken = default);
    }
}