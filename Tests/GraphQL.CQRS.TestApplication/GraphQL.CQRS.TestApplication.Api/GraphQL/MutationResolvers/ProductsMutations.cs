using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using HotChocolate;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationResolver", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.MutationResolvers
{
    [ExtendObjectType(Name = "Mutation")]
    public class ProductsMutations
    {
        public async Task<Guid> CreateProduct(ProductCreateDto dto, [Service] IProductsService service)
        {
            return await service.CreateProduct(dto);
        }

        public async Task<ProductDto> FindProductById(Guid id, [Service] IProductsService service)
        {
            return await service.FindProductById(id);
        }

        public async Task<IReadOnlyList<ProductDto>> FindProducts([Service] IProductsService service)
        {
            return await service.FindProducts();
        }

        [UseMutationConvention]
        public async Task UpdateProduct(Guid id, ProductUpdateDto dto, [Service] IProductsService service)
        {
            await service.UpdateProduct(id, dto);
        }

        public async Task DeleteProduct(Guid id, [Service] IProductsService service)
        {
            await service.DeleteProduct(id);
        }
    }
}