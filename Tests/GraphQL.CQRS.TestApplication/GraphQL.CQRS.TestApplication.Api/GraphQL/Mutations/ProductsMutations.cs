using System;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using HotChocolate;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationResolver", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Mutations
{
    [ExtendObjectType(Name = "Mutation")]
    public class ProductsMutations
    {
        public async Task<Guid> CreateProduct(ProductCreateDto dto, [Service] IProductsService service)
        {
            return await service.CreateProduct(dto);
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