using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Invoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoiceById;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesForCustomer;
using HotChocolate;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.QueryResolver", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.QueryResolvers
{
    [ExtendObjectType(Name = "Query")]
    public class InvoiceQueries
    {
        public async Task<InvoiceDto> GetInvoiceById(
            GetInvoiceByIdQuery input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetInvoicesForCustomer(
            GetInvoicesForCustomerQuery input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetInvoices(
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(new GetInvoicesQuery(), cancellationToken);
        }
    }
}