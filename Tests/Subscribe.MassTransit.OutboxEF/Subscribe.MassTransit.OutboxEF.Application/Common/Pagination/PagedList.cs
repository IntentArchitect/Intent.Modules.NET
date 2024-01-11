using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Subscribe.MassTransit.OutboxEF.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.PagedList", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application.Common.Pagination
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(int totalCount, int pageNo, int pageSize, IEnumerable<T> results)
        {
            TotalCount = totalCount;
            PageCount = GetPageCount(pageSize, TotalCount);
            PageNo = pageNo;
            PageSize = pageSize;
            AddRange(results);
        }

        public int TotalCount { get; private set; }
        public int PageCount { get; private set; }
        public int PageNo { get; private set; }
        public int PageSize { get; private set; }

        private int GetPageCount(int pageSize, int totalCount)
        {
            if (pageSize == 0)
            {
                return 0;
            }
            var remainder = totalCount % pageSize;
            return (totalCount / pageSize) + (remainder == 0 ? 0 : 1);
        }
    }
}