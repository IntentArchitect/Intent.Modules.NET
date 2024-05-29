using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.Unversioned;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices
{
    public interface ITestUnversionedProxy : IDisposable
    {
        Task TestAsync(TestCommand command, CancellationToken cancellationToken = default);
        Task<int> TestAsync(string value, CancellationToken cancellationToken = default);
    }
}