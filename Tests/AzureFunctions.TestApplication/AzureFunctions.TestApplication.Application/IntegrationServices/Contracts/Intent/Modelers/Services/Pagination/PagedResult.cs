using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Intent.Modelers.Services.Pagination
{
    public class PagedResult<TData>
    {
        public PagedResult()
        {
            TotalCount = null!;
            PageSize = null!;
            Data = null!;
        }

        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public string TotalCount { get; set; }
        public string PageSize { get; set; }
        public List<TData> Data { get; set; }

        public static PagedResult<TData> Create(
            int pageNumber,
            int pageCount,
            string totalCount,
            string pageSize,
            List<TData> data)
        {
            return new PagedResult<TData>
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                TotalCount = totalCount,
                PageSize = pageSize,
                Data = data
            };
        }
    }
}