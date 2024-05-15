using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.CustomRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories
{
    public interface ICustomAppDefaultRepository
    {
        Task TestProc(CancellationToken cancellationToken = default);
    }
}