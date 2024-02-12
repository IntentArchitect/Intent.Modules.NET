using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Customers
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