using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.NET6.Application.IntegrationServices
{
    public interface INullableResultService : IDisposable
    {
        Task<CustomerDto> GetCustomerNullableAsync(CancellationToken cancellationToken = default);
    }
}