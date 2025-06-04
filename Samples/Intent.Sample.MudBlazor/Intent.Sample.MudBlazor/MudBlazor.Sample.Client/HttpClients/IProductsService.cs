using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Client.HttpClients.Common;
using MudBlazor.Sample.Client.HttpClients.Contracts.Services.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients
{
    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken = default);
        Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> GetProductsAsync(int pageNo, int pageSize, string? orderBy, string? searchText, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> GetProductsLookupAsync(CancellationToken cancellationToken = default);
    }
}