using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Common.Pagination;
using GraphQL.CQRS.TestApplication.Application.Invoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoiceById;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesForCustomer;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesPaged;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryType", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.Queries
{
    [ExtendObjectType(OperationType.Query)]
    public class InvoiceQueries
    {
        public async Task<InvoiceDto> GetInvoiceById(
            Guid id,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoiceByIdQuery { Id = id }, cancellationToken);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetInvoicesForCustomer(
            Guid customerId,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesForCustomerQuery { CustomerId = customerId }, cancellationToken);
        }

        [GraphQLDescription("Returns the paged result of invoices")]
        public async Task<PagedResult<InvoiceDto>> GetInvoicesPaged(
            [GraphQLDescription("The page index")] int pageIndex,
            [GraphQLDescription("The page size")] string pageSize,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesPagedQuery { PageIndex = pageIndex, PageSize = pageSize }, cancellationToken);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetInvoices(
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesQuery(), cancellationToken);
        }
    }
}