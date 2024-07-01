using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Customers;
using OpenApiImporterTest.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace OpenApiImporterTest.Application.Implementation
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
            // TODO: Implement CreateCustomer (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerDto>> GetCustomers(CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetCustomers (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteCustomer (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateCustomer(UpdateCustomerCommand dto, Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateCustomer (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerDto> GetCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetCustomer (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateOrder(
            CreateCustomerOrderCommand dto,
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateOrder (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerOrderDto>> GetOrders(Guid customerId, CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetOrders (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteOrder(Guid customerId, Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteOrder (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateOrder(
            UpdateCustomerOrderCommand dto,
            Guid customerId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateOrder (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerOrderDto> GetOrder(Guid customerId, Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetOrder (CustomerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}