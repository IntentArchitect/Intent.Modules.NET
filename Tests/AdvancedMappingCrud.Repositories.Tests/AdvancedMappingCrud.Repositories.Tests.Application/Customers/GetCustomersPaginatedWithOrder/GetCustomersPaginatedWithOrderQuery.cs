using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersPaginatedWithOrder
{
    public class GetCustomersPaginatedWithOrderQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersPaginatedWithOrderQuery(bool isActive,
            string name,
            string surname,
            int pageNo,
            int pageSize,
            string orderBy)
        {
            IsActive = isActive;
            Name = name;
            Surname = surname;
            PageNo = pageNo;
            PageSize = pageSize;
            OrderBy = orderBy;
        }

        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
    }
}