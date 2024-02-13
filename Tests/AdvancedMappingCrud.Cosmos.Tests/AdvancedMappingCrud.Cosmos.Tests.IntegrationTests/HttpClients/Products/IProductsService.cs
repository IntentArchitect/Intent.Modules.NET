using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Products
{
    public interface IProductsService : IDisposable
    {
        Task<string> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(string id, UpdateProductCommand command, CancellationToken cancellationToken = default);
        Task<ProductDto> GetProductByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}