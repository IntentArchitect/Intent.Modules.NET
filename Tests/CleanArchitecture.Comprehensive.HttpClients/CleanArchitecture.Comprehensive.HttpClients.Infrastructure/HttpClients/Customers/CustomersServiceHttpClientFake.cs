using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.Fakes.HttpClientFake", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.HttpClients.Customers
{
    public class CustomersServiceHttpClientFake : ICustomersService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateCustomerAsync(
            CreateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(Guid.Empty);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CustomerDtoFactory.CreateDefault());
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> GetCustomerByNameEmailAsync(
            GetCustomerByNameEmailQuery query,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CustomerDtoFactory.CreateDefaultList(1));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> GetCustomerExtraFieldsAsync(
            GetCustomerExtraFieldsQuery query,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CustomerDtoFactory.CreateDefaultList(1));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CustomerDtoFactory.CreateDefaultList(1));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}