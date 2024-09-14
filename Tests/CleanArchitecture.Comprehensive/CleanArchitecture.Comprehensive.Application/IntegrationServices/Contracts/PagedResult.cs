using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.PagedResult", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices.Contracts
{
    public class PagedResult<TData>
    {
        public PagedResult()
        {
            Data = null!;
        }

        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<TData> Data { get; set; }
    }
}