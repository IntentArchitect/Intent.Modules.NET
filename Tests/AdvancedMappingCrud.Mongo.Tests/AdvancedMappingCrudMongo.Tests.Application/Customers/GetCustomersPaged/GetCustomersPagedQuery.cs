using System.ComponentModel;
using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Customers.GetCustomersPaged
{
    public class GetCustomersPagedQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersPagedQuery(int pageNo = 5, int pageSize = 50)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        [DefaultValue(5)]
        public int PageNo { get; set; }
        [DefaultValue(50)]
        public int PageSize { get; set; }
    }
}