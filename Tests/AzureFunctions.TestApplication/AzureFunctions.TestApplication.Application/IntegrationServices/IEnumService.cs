using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface IEnumService : IDisposable
    {
        Task TestRouteEnumAsync(Company testEnum, CancellationToken cancellationToken = default);
        Task TestQueryEnumAsync(Company testEnum, CancellationToken cancellationToken = default);
        Task TestHeaderEnumAsync(Company testEnum, CancellationToken cancellationToken = default);
    }
}