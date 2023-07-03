using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.TestUnversionedProxy
{
    public interface ITestUnversionedProxyClient : IDisposable
    {
        Task TestAsync(TestCommand command, CancellationToken cancellationToken = default);
        Task<int> TestAsync(string value, CancellationToken cancellationToken = default);
    }
}