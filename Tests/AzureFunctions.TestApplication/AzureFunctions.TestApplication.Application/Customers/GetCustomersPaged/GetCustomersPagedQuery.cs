using AzureFunctions.TestApplication.Application.Common.Interfaces;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Intent.Modelers.Services.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Customers.GetCustomersPaged
{
    public class GetCustomersPagedQuery : IRequest<AzureFunctions.TestApplication.Application.Common.Pagination.PagedResult<CustomerDto>>, IQuery
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