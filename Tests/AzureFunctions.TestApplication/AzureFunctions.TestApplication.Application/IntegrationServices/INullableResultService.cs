using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface INullableResultService : IDisposable
    {
        Task<CustomerDto> GetCustomerNullableAsync(CancellationToken cancellationToken = default);
    }
}