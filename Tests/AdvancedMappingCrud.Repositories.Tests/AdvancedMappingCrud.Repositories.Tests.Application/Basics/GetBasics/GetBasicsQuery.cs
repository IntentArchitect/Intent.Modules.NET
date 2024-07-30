using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Basics.GetBasics
{
    public class GetBasicsQuery : IRequest<PagedResult<BasicDto>>, IQuery
    {
        public GetBasicsQuery(int pageNo, int pageSize, string orderBy)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
    }
}