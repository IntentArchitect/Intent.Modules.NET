using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.GetAggregateRootLongs
{
    public class GetAggregateRootLongsQuery : IRequest<PagedResult<AggregateRootLongDto>>, IQuery
    {
        public GetAggregateRootLongsQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}