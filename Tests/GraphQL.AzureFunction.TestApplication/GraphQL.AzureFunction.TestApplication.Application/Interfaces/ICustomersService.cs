using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.AzureFunction.TestApplication.Application.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Interfaces
{
    public interface ICustomersService : IDisposable
    {
        Task<CustomerDto> CreateCustomer(CustomerCreateDto dto, CancellationToken cancellationToken = default);
        Task<CustomerDto> FindCustomerById(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> FindCustomers(CancellationToken cancellationToken = default);
        Task<CustomerDto> UpdateCustomer(Guid id, CustomerUpdateDto dto, CancellationToken cancellationToken = default);
        Task<CustomerDto> DeleteCustomer(Guid id, CancellationToken cancellationToken = default);
    }
}