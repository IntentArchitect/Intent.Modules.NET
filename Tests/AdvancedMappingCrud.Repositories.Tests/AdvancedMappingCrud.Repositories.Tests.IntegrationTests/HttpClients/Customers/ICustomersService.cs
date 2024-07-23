using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Customers
{
    public interface ICustomersService : IDisposable
    {
        Task ApproveQuoteAsync(Guid quoteId, ApproveQuoteCommand command, CancellationToken cancellationToken = default);
        Task CreateCorporateFuneralCoverQuoteAsync(CreateCorporateFuneralCoverQuoteCommand command, CancellationToken cancellationToken = default);
        Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default);
        Task CreateFuneralCoverQuoteAsync(CreateFuneralCoverQuoteCommand command, CancellationToken cancellationToken = default);
        Task CreateQuoteAsync(CreateQuoteCommand command, CancellationToken cancellationToken = default);
        Task DeactivateCustomerAsync(DeactivateCustomerCommand command, CancellationToken cancellationToken = default);
        Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateCorporateFuneralCoverQuoteAsync(Guid id, UpdateCorporateFuneralCoverQuoteCommand command, CancellationToken cancellationToken = default);
        Task UpdateCustomerAsync(Guid id, UpdateCustomerCommand command, CancellationToken cancellationToken = default);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetCustomersPaginatedAsync(bool isActive, string name, string surname, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<CustomerDto>> GetCustomersPaginatedWithOrderAsync(bool isActive, string name, string surname, int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default);
        Task<int> GetCustomerStatisticsAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<List<CustomerDto>> GetCustomersWithParamsAsync(bool isActive, string? name, string? surname, CancellationToken cancellationToken = default);
    }
}