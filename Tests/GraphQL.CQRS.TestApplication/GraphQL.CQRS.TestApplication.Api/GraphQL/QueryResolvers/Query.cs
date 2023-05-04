using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Customers;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomerById;
using GraphQL.CQRS.TestApplication.Application.Customers.GetCustomers;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Invoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoices;
using GraphQL.CQRS.TestApplication.Application.Products;
using HotChocolate;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryResolver", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.QueryResolvers
{
    [ExtendObjectType(Name = "Query")]
    public class Query
    {
        public async Task<IReadOnlyList<CustomerDto>> GetCustomers(
            GetCustomersQuery input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<CustomerDto> GetCustomersById(
            Guid id,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetCustomerByIdQuery { Id = id }, cancellationToken);
        }

        public async Task<IReadOnlyList<ProductDto>> GetProducts([Service] IProductsService service)
        {
            return await service.FindProducts();
        }

        public async Task<ProductDto> GetProductById(Guid id, [Service] IProductsService service)
        {
            return await service.FindProductById(id);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetInvoices(
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesQuery(), cancellationToken);
        }
    }
}