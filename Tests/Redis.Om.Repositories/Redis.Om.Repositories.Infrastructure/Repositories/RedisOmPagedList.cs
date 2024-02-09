using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Repositories;
using Redis.Om.Repositories.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmPagedList", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Repositories
{
    internal class RedisOmPagedList<TDomain, TDocument> : List<TDomain>, IPagedList<TDomain>
        where TDomain : class
        where TDocument : IRedisOmDocument<TDomain, TDocument>
    {
        public RedisOmPagedList(int totalCount, int pageNo, int pageSize, IEnumerable<TDomain> results)
        {
            TotalCount = totalCount;
            PageCount = GetPageCount(pageSize, totalCount);
            PageNo = pageNo;
            PageSize = pageSize;

            foreach (var result in results)
            {
                Add(result);
            }
        }

        public int TotalCount { get; }
        public int PageCount { get; }
        public int PageNo { get; }
        public int PageSize { get; }

        private static int GetPageCount(int pageSize, int totalCount)
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