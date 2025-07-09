using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.CQRS.TestApplication.Application.Common.Pagination;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInvoicesPagedQueryHandler : IRequestHandler<GetInvoicesPagedQuery, PagedResult<InvoiceDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetInvoicesPagedQueryHandler()
        {
        }

        /// <summary>
        /// Returns the paged result of invoices
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<PagedResult<InvoiceDto>> Handle(
            GetInvoicesPagedQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetInvoicesPagedQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}