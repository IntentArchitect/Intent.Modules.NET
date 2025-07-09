using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ProductsService : IProductsService
    {
        [IntentManaged(Mode.Merge)]
        public ProductsService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<ProductDto> CreateProduct(ProductCreateDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateProduct (ProductsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<ProductDto> FindProductById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindProductById (ProductsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<ProductDto>> FindProducts(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindProducts (ProductsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<ProductDto> UpdateProduct(
            Guid id,
            ProductUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateProduct (ProductsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<ProductDto> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteProduct (ProductsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}