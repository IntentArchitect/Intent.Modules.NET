using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Invoices;
using GraphQL.CQRS.TestApplication.Application.Invoices.CreateInvoice;
using GraphQL.CQRS.TestApplication.Application.Invoices.DeleteInvoice;
using GraphQL.CQRS.TestApplication.Application.Invoices.UpdateInvoice;
using HotChocolate;
using HotChocolate.Types;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.MutationResolver", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Api.GraphQL.MutationResolvers
{
    [ExtendObjectType(Name = "Mutation")]
    public class InvoiceMutations
    {
        public async Task<Guid> CreateInvoice(
            CreateInvoiceCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<InvoiceDto> DeleteInvoice(
            DeleteInvoiceCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }

        public async Task<InvoiceDto> UpdateInvoice(
            UpdateInvoiceCommand input,
            CancellationToken cancellationToken,
            [Service] ISender mediator)
        {
            return await mediator.Send(input, cancellationToken);
        }
    }
}