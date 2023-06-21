using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.AzureFunction.TestApplication.Application.Products;
using GraphQL.AzureFunction.TestApplication.Application.Products.CreateProduct;
using GraphQL.AzureFunction.TestApplication.Application.Products.DeleteProduct;
using GraphQL.AzureFunction.TestApplication.Application.Products.UpdateProduct;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationType", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api.GraphQL.Mutations
{
    [ExtendObjectType(OperationType.Mutation)]
    public class ProductMutations
    {
        public async Task<ProductDto> CreateProduct(
            CreateProductCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<ProductDto> DeleteProduct(
            DeleteProductCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<ProductDto> UpdateProduct(
            UpdateProductCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }
    }
}