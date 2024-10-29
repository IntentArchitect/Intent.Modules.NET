using System;
using AzureFunctions.NET6.Application.Common.Interfaces;
using AzureFunctions.NET6.Application.Common.Pagination;
using AzureFunctions.NET8.Application.Common.Interfaces;
using AzureFunctions.NET8.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Customers.GetPagedWithParameters
{
    public class GetPagedWithParameters : IRequest<PagedResult<CustomerDto>>, IQuery
    {
        public GetPagedWithParameters(int pageNo, int pageSize, string searchCriteria, Guid id)
        {
            PageNo = pageNo;
            PageSize = pageSize;
            SearchCriteria = searchCriteria;
            Id = id;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string SearchCriteria { get; set; }
        public Guid Id { get; set; }
    }
}