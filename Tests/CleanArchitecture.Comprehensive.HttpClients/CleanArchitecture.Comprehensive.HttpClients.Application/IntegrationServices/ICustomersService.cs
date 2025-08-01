using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices
{
    public interface ICustomersService : IDisposable
    {
        Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default);
    }
}