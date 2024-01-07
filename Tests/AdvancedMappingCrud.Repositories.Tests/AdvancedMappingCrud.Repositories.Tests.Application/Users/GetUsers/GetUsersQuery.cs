using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUsers
{
    public class GetUsersQuery : IRequest<PagedResult<UserDto>>, IQuery
    {
        public GetUsersQuery(string? name, string? surname, int pageNo, int pageSize)
        {
            Name = name;
            Surname = surname;
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}