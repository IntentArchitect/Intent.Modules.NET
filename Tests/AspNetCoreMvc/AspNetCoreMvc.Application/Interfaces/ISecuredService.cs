using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AspNetCoreMvc.Application.Interfaces
{
    public interface ISecuredService
    {
        Task Operation(CancellationToken cancellationToken = default);
    }
}