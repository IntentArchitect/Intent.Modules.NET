using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.IntegrationServices
{
    public interface IDeleteAccountsService : IDisposable
    {
        Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken = default);
    }
}