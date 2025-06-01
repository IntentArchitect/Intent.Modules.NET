using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Pagination.GetPeopleByNullableFirstNamePaginated
{
    public class GetPeopleByNullableFirstNamePaginatedQuery : IRequest<PagedResult<PersonEntryDto>>, IQuery
    {
        public GetPeopleByNullableFirstNamePaginatedQuery(string? firstName, int pageNo, int pageSize, string? orderBy)
        {
            FirstName = firstName;
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public string? FirstName { get; set; }
        [QueryParam]
        public int PageNo { get; set; }
        [QueryParam]
        public int PageSize { get; set; }
        [QueryParam]
        public string? OrderBy { get; set; }
    }
}