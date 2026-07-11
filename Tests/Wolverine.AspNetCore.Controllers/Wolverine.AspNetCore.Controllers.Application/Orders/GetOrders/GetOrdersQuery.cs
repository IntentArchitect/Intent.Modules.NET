using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.GetOrders
{
    /// <summary>
    /// Paged get-all. Convention-based (no domain interaction) so CQRS.CRUD's paged strategy implements it.
    /// </summary>
    public class GetOrdersQuery : IQuery
    {
        public GetOrdersQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}