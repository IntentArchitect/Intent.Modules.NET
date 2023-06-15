using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.MultiVersionServiceProxy
{
    public interface IMultiVersionServiceProxyClient : IDisposable
    {
        Task OperationForVersionOneAsync(CancellationToken cancellationToken = default);
        Task OperationForVersionTwoAsync(CancellationToken cancellationToken = default);
    }
}