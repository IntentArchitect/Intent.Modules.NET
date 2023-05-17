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
        Task<ProductDto> CreateProduct(ProductCreateDto dto);
        Task<ProductDto> FindProductById(Guid id);
        Task<List<ProductDto>> FindProducts();
        Task<ProductDto> UpdateProduct(Guid id, ProductUpdateDto dto);
        Task<ProductDto> DeleteProduct(Guid id);

    }
}