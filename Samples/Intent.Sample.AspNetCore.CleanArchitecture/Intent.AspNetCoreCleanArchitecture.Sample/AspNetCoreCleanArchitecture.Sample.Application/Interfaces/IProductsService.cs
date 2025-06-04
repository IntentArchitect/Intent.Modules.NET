using AspNetCoreCleanArchitecture.Sample.Application.Common.Pagination;
using AspNetCoreCleanArchitecture.Sample.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Interfaces
{
    public interface IProductsService
    {
        Task<Guid> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateProduct(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto> FindProductById(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<ProductDto>> FindProducts(int pageNo, int pageSize, string? orderBy, CancellationToken cancellationToken = default);
        Task DeleteProduct(Guid id, CancellationToken cancellationToken = default);
    }
}