using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.CreateCustomer;
using GraphQL.CQRS.TestApplication.Application.Customers.UpdateCustomer;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Invoices.CreateInvoice;
using GraphQL.CQRS.TestApplication.Application.Products;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Mutations
{
    [ExtendObjectType(OperationType.Mutation)]
    public class Mutation
    {
        public async Task<CustomerDto> CreateCustomer(
            CreateCustomerCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<Guid> CreateProduct(ProductCreateDto input, [Service] IProductsService service)
        {
            return await service.CreateProduct(input);
        }

        public async Task<Guid> CreateInvoice(
            CreateInvoiceCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<CustomerDto> UpdateCustomer(
            UpdateCustomerCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }
    }
}