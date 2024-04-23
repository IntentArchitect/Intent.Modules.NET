using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Import.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace OpenApiImporterTest.Application.Interfaces.Import
{
    public interface ICustomerService : IDisposable
    {
        Task<Guid> CreateCustomer(CreateCustomerCommand dto, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomers(CancellationToken cancellationToken = default);
        Task<Guid> CreateCustomerOrder(CreateCustomerOrderCommand dto, Guid customerId, CancellationToken cancellationToken = default);
        Task<List<CustomerOrderDto>> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken = default);
        Task DeleteCustomer(Guid id, CancellationToken cancellationToken = default);
        Task UpdateCustomer(UpdateCustomerCommand dto, Guid id, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomer(Guid id, CancellationToken cancellationToken = default);
        Task DeleteCustomerOrder(Guid customerId, Guid id, CancellationToken cancellationToken = default);
        Task UpdateCustomerOrder(UpdateCustomerOrderCommand dto, Guid customerId, Guid id, CancellationToken cancellationToken = default);
        Task<CustomerOrderDto> GetCustomerOrder(Guid customerId, Guid id, CancellationToken cancellationToken = default);
    }
}