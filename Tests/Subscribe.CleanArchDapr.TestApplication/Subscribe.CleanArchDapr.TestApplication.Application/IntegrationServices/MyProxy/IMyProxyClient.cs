using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.MyProxy
{
    public interface IMyProxyClient : IDisposable
    {
        Task OrderConfirmedAsync(Guid id, OrderConfirmed command, CancellationToken cancellationToken = default);
    }
}