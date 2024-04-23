using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Import.Customers;
using OpenApiImporterTest.Application.Interfaces.Import;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace OpenApiImporterTest.Application.Implementation.Import
{
    [IntentManaged(Mode.Merge)]
    public class CustomerService : ICustomerService
    {
        [IntentManaged(Mode.Merge)]
        public CustomerService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateCustomer(CreateCustomerCommand dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerDto>> GetCustomers(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateCustomerOrder(
            CreateCustomerOrderCommand dto,
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerOrderDto>> GetCustomerOrders(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateCustomer(UpdateCustomerCommand dto, Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerDto> GetCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteCustomerOrder(Guid customerId, Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateCustomerOrder(
            UpdateCustomerOrderCommand dto,
            Guid customerId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerOrderDto> GetCustomerOrder(
            Guid customerId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}