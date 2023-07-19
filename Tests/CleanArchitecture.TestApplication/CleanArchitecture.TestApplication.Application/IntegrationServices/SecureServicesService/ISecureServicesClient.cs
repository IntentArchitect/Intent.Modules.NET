using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.SecureServicesService
{
    public interface ISecureServicesClient : IDisposable
    {
        Task SecureAsync(SecureCommand command, CancellationToken cancellationToken = default);
    }
}