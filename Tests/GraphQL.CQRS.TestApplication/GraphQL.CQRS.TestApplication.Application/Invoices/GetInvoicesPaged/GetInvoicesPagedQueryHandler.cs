using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoicesPagedQueryHandler : IRequestHandler<GetInvoicesPagedQuery, PagedResult<InvoiceDto>>
    {
        [IntentManaged(Mode.Ignore)]
        public GetInvoicesPagedQueryHandler()
        {
        }

        /// <summary>
        /// Returns the paged result of invoices
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<InvoiceDto>> Handle(
            GetInvoicesPagedQuery request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}