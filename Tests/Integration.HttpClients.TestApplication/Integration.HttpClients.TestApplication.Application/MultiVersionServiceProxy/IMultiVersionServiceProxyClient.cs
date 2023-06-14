using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.MultiVersionServiceProxy
{
    public interface IMultiVersionServiceProxyClient : IDisposable
    {
        Task OperationForVersionOneAsync(CancellationToken cancellationToken = default);
        Task OperationForVersionTwoAsync(CancellationToken cancellationToken = default);
    }
}