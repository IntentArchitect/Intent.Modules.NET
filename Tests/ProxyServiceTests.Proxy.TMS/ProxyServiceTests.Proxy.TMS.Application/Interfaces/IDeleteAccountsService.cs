using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.Interfaces
{
    public interface IDeleteAccountsService : IDisposable
    {
        Task DeleteAccountCommand(Guid id, CancellationToken cancellationToken = default);
    }
}