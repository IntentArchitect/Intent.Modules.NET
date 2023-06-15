using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.VersionOneServiceProxy
{
    public interface IVersionOneServiceProxyClient : IDisposable
    {
        Task OperationForVersionOneAsync(string param, CancellationToken cancellationToken = default);
    }
}