using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Client.HttpClients.Common;
using MudBlazor.Sample.Client.HttpClients.Contracts.Services.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients
{
    public interface ICustomersService : IDisposable
    {
        Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetCustomersAsync(int pageNo, int pageSize, string? orderBy, string? searchText, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(Guid id, UpdateCustomerCommand command, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerLookupDto>> GetCustomersLookupAsync(CancellationToken cancellationToken = default);
    }
}