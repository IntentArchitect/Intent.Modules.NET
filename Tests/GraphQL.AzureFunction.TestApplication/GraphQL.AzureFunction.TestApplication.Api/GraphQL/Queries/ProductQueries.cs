using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.AzureFunction.TestApplication.Application.Products;
using GraphQL.AzureFunction.TestApplication.Application.Products.GetProductById;
using GraphQL.AzureFunction.TestApplication.Application.Products.GetProducts;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class ProductQueries
    {
        public async Task<ProductDto> GetProductById(
            Guid id,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetProductByIdQuery(id: id), cancellationToken);
        }

        public async Task<IReadOnlyList<ProductDto>> GetProducts(
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetProductsQuery(), cancellationToken);
        }
    }
}