using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Interfaces
{

    public interface IProductsService : IDisposable
    {
        Task<ProductDto> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto> FindProductById(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductDto>> FindProducts(CancellationToken cancellationToken = default);
        Task<ProductDto> UpdateProduct(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task<ProductDto> DeleteProduct(Guid id, CancellationToken cancellationToken = default);

    }
}