using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Interfaces
{
    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto> FindProductById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> FindProducts(CancellationToken cancellationToken = default);
        Task UpdateProduct(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteProduct(Guid id, CancellationToken cancellationToken = default);
        Task<decimal> GetProductPrice(Guid productId, decimal prices, CancellationToken cancellationToken = default);
    }
}