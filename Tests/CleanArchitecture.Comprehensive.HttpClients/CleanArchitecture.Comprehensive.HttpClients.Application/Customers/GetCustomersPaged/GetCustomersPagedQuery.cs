using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.GetCustomersPaged
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