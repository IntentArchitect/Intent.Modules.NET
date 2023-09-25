using System.Collections.Generic;
using CosmosDBMultiTenancy.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosPagedList", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure.Repositories
{
    internal class CosmosPagedList<TDomain, TDocument> : List<TDomain>, IPagedResult<TDomain>
        where TDocument : TDomain, IItem
    {
        public CosmosPagedList(IPageQueryResult<TDocument> pagedResult, int pageNo, int pageSize)
        {
            TotalCount = pagedResult.Total ?? 0;
            PageCount = pagedResult.TotalPages ?? 0;
            PageNo = pageNo;
            PageSize = pageSize;

            foreach (var result in pagedResult.Items)
            {
                Add(result);
            }
        }

        public int TotalCount { get; private set; }
        public int PageCount { get; private set; }
        public int PageNo { get; private set; }
        public int PageSize { get; private set; }
    }
}