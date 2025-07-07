using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients
{
    public interface ICustomersService : IDisposable
    {
        Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(GetCustomerByIdQuery query, CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetCustomersAsync(GetCustomersQuery query, CancellationToken cancellationToken = default);
        Task<List<CustomerLookupDto>> GetCustomersLookupAsync(CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default);
    }
}