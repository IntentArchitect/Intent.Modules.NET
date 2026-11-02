using HttpQuery.Client.Application.IntegrationServices.Contracts.Controllers.Swashbuckle.Services.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace HttpQuery.Client.Application.IntegrationServices.Products
{
    public interface IProductsProxy : IDisposable
    {
        Task<Guid> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);
        Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> SearchProductsAsync(SearchProductsQuery query, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> SearchCategoryProductsAsync(string category, SearchCategoryProductsQuery query, CancellationToken cancellationToken = default);
    }
}