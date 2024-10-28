using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Pagination.GetLogEntries
{
    public class GetLogEntriesQuery : IRequest<PagedResult<LogEntryDto>>, IQuery
    {
        public GetLogEntriesQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        [FromQueryParams]
        public int PageNo { get; set; }
        [FromQueryParams]
        public int PageSize { get; set; }
    }
}