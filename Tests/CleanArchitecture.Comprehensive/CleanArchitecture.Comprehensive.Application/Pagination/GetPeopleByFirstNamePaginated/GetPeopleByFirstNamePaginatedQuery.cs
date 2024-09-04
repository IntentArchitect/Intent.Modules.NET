using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination.GetPeopleByFirstNamePaginated
{
    public class GetPeopleByFirstNamePaginatedQuery : IRequest<PagedResult<PersonEntryDto>>, IQuery
    {
        public GetPeopleByFirstNamePaginatedQuery(string firstName, int pageNo, int pageSize, string? orderBy)
        {
            FirstName = firstName;
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public string FirstName { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
    }
}