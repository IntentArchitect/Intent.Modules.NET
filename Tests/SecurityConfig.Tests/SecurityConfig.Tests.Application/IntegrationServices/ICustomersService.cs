using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SecurityConfig.Tests.Application.IntegrationServices.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices
{
    public interface ICustomersService : IDisposable
    {
        Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(Guid id, UpdateCustomerCommand command, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default);
    }
}