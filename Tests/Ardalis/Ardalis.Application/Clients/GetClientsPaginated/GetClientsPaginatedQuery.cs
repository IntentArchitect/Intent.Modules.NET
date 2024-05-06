using Ardalis.Application.Common.Interfaces;
using Ardalis.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Ardalis.Application.Clients.GetClientsPaginated
{
    public class GetClientsPaginatedQuery : IRequest<PagedResult<ClientDto>>, IQuery
    {
        public GetClientsPaginatedQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}