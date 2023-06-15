using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy
{
    public interface ITestVersionedProxyClient : IDisposable
    {
        Task TestCommandV1Async(TestCommandV1 command, CancellationToken cancellationToken = default);
        Task TestCommandV2Async(TestCommandV2 command, CancellationToken cancellationToken = default);
        Task<int> TestQueryV1Async(TestQueryV1 query, CancellationToken cancellationToken = default);
        Task<int> TestQueryV2Async(TestQueryV2 query, CancellationToken cancellationToken = default);
    }
}