using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.Stubs.HttpClientStub", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.Stubs.HttpClients.Customers
{
    public class CustomersServiceHttpClientStub : ICustomersService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateCustomerAsync(
            CreateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(Guid.Empty);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new CustomerDto
            {
                Id = Guid.Empty,
                Email = string.Empty,
                Name = string.Empty,
                Surname = string.Empty
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerDto>> GetCustomerByNameEmailAsync(
            GetCustomerByNameEmailQuery query,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.Empty,
                    Email = string.Empty,
                    Name = string.Empty,
                    Surname = string.Empty
                }
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerDto>> GetCustomerExtraFieldsAsync(
            GetCustomerExtraFieldsQuery query,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.Empty,
                    Email = string.Empty,
                    Name = string.Empty,
                    Surname = string.Empty
                }
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.Empty,
                    Email = string.Empty,
                    Name = string.Empty,
                    Surname = string.Empty
                }
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<CustomerDto>> GetCustomersPagedAsync(
            GetCustomersPagedQuery query,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new PagedResult<CustomerDto>
            {
                TotalCount = 1,
                PageCount = 1,
                PageSize = query.PageSize,
                PageNumber = query.PageNo,
                Data = new List<CustomerDto>
                {
                    new CustomerDto
                    {
                        Id = Guid.Empty,
                        Email = string.Empty,
                        Name = string.Empty,
                        Surname = string.Empty
                    }
                }
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}