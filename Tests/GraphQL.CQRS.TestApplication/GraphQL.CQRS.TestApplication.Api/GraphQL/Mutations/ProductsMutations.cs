using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Mutations
{
    [ExtendObjectType(OperationType.Mutation)]
    public class ProductsMutations
    {
        public async Task<ProductDto> CreateProduct(ProductCreateDto dto, [Service] IProductsService service)
        {
            return await service.CreateProduct(dto);
        }

        [UseMutationConvention]
        public async Task<ProductDto> UpdateProduct(Guid id, ProductUpdateDto dto, [Service] IProductsService service)
        {
            return await service.UpdateProduct(id, dto);
        }

        public async Task<ProductDto> DeleteProduct(Guid id, [Service] IProductsService service)
        {
            return await service.DeleteProduct(id);
        }
    }
}