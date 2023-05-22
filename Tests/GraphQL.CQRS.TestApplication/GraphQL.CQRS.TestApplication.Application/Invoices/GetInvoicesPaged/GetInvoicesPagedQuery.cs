using System;
using System.Collections.Generic;
using GraphQL.CQRS.TestApplication.Application.Common.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.GetInvoicesPaged
{
    /// <summary>
    /// Returns the paged result of invoices
    /// </summary>
    public class GetInvoicesPagedQuery : IRequest<PagedResult<InvoiceDto>>, IQuery
    {
        public GetInvoicesPagedQuery(int pageIndex, string pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
        public int PageIndex { get; set; }

        public string PageSize { get; set; }

    }
}