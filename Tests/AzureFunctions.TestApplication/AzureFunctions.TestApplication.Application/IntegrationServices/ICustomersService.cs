using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices
{
    public interface ICustomersService : IDisposable
    {
        Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(Guid id, UpdateCustomerCommand command, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetCustomersPagedAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetPagedWithParametersAsync(int pageNo, int pageSize, string searchCriteria, Guid id, CancellationToken cancellationToken = default);
    }
}