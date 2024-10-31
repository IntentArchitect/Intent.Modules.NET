using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Interfaces
{
    public interface IListedUnlistedServicesService
    {
        Task ListedServiceFunc(string param, CancellationToken cancellationToken = default);
        Task UnlistedServiceFunc(CancellationToken cancellationToken = default);
    }
}