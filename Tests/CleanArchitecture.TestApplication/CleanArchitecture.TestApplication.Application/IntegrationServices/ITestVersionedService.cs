using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Application.IntegrationServices.CleanArchitecture.TestApplication.Services.Versioned;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices
{
    public interface ITestVersionedService : IDisposable
    {
        Task TestCommandV1Async(TestCommandV1 command, CancellationToken cancellationToken = default);
        Task TestCommandV2Async(TestCommandV2 command, CancellationToken cancellationToken = default);
        Task<int> TestQueryV1Async(string value, CancellationToken cancellationToken = default);
        Task<int> TestQueryV2Async(string value, CancellationToken cancellationToken = default);
    }
}