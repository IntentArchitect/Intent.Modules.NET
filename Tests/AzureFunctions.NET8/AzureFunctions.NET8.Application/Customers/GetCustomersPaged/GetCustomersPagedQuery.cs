using AzureFunctions.NET6.Application.Common.Interfaces;
using AzureFunctions.NET6.Application.Common.Pagination;
using AzureFunctions.NET8.Application.Common.Interfaces;
using AzureFunctions.NET8.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Customers.GetCustomersPaged
{
    public class GetCustomersPagedQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersPagedQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}