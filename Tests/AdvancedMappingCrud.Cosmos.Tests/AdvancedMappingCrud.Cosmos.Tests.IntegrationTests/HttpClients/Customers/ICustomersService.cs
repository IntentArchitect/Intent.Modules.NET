using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Customers
{
    public interface ICustomersService : IDisposable
    {
        Task<string> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(string id, UpdateCustomerCommand command, CancellationToken cancellationToken = default);
        Task<CustomerDto> FindCustomerByNameOrSurnameAsync(string? name, string? surname, CancellationToken cancellationToken = default);
        Task<CustomerDto> FindCustomerByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersLinqAsync(CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetCustomersPagedAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default);
    }
}