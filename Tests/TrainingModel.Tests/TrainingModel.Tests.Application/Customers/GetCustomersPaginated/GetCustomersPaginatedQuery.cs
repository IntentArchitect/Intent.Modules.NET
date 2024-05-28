using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;
using TrainingModel.Tests.Application.Common.Pagination;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Customers.GetCustomersPaginated
{
    public class GetCustomersPaginatedQuery : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetCustomersPaginatedQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}