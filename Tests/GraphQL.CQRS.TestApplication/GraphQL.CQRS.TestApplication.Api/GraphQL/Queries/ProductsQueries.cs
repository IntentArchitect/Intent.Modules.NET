using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class ProductsQueries
    {

        public async Task<ProductDto> FindProductById(Guid id, [Service] IProductsService service)
        {
            return await service.FindProductById(id);
        }

        public async Task<IReadOnlyList<ProductDto>> FindProducts([Service] IProductsService service)
        {
            return await service.FindProducts();
        }
    }
}