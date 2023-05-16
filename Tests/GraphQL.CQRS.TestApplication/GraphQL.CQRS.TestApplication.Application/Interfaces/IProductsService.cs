using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Interfaces
{

    public interface IProductsService : IDisposable
    {
        Task<Guid> CreateProduct(ProductCreateDto dto);
        Task<ProductDto> FindProductById(Guid id);
        Task<List<ProductDto>> FindProducts();
        Task UpdateProduct(Guid id, ProductUpdateDto dto);
        Task DeleteProduct(Guid id);

    }
}