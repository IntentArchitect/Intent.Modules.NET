using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.AzureFunction.TestApplication.Application.Common.Pagination;
using GraphQL.AzureFunction.TestApplication.Application.Customers;
using GraphQL.AzureFunction.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CustomersService : ICustomersService
    {
        [IntentManaged(Mode.Merge)]
        public CustomersService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> CreateCustomer(CustomerCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCustomer (CustomersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> FindCustomerById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindCustomerById (CustomersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<CustomerDto>> FindCustomers(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindCustomers (CustomersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> UpdateCustomer(
            Guid id,
            CustomerUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateCustomer (CustomersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> DeleteCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteCustomer (CustomersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<PagedResult<CustomerDto>> GetCustomersPaged(
            int pageNo,
            int pageCount,
            List<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetCustomersPaged (CustomersService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}