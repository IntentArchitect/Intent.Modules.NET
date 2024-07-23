using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Products
{
    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken = default);
        Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsPaginatedByNameOptionalAsync(string? name, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsPaginatedByNameOptionalWithOrderAsync(string? name, int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsPaginatedByNameAsync(string name, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsPaginatedByNameWithOrderAsync(string name, int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsPaginatedAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsPaginatedWithOrderAsync(int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}