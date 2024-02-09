using IntegrationTesting.Tests.IntegrationTests.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients
{
    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto> FindProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> FindProductsAsync(CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    }
}