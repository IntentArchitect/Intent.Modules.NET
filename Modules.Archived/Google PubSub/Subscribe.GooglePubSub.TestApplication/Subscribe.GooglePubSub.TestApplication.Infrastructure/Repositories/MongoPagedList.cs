using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoFramework;
using MongoFramework.Linq;
using Subscribe.GooglePubSub.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.PagedList", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Infrastructure.Repositories
{
    public class MongoPagedList<T> : List<T>, IPagedList<T>
    {
        public MongoPagedList(IQueryable<T> source, int pageNo, int pageSize)
        {
            TotalCount = source.Count();
            PageCount = GetPageCount(pageSize, TotalCount);
            PageNo = pageNo;
            PageSize = pageSize;
            var skip = ((PageNo - 1) * PageSize);

            AddRange(
                source
                    .Skip(skip)
                    .Take(PageSize)
                    .ToList());
        }

        public MongoPagedList(int totalCount, int pageNo, int pageSize, List<T> results)
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

        public static async Task<IPagedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var skip = ((pageNo - 1) * pageSize);

            var results = await source
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return new MongoPagedList<T>(count, pageNo, pageSize, results);
        }
    }
}