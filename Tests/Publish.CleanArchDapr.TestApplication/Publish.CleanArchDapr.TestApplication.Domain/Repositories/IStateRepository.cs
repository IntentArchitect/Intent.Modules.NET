using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.StateRepositoryInterface", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Domain.Repositories
{
    public interface IStateRepository
    {
        void Update<T>(string id, T state);
        Task<T> Get<T>(string id);
        void Delete(string id);
        Task FlushAllAsync(CancellationToken cancellationToken = default);
    }
}