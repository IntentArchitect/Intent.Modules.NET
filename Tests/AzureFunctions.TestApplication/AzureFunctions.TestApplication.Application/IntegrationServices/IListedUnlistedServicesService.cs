using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface IListedUnlistedServicesService : IDisposable
    {
        Task ListedServiceFuncAsync(string param, CancellationToken cancellationToken = default);
    }
}