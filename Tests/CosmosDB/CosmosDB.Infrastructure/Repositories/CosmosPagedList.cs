using System.Collections.Generic;
using CosmosDB.Domain.Repositories;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosPagedList", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal class CosmosPagedList<TDomain, TDocument> : List<TDomain>, IPagedResult<TDomain>
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>
    {
        public CosmosPagedList(IPageQueryResult<TDocument> pagedResult, int pageNo, int pageSize)
        {
            TotalCount = pagedResult.Total ?? 0;
            PageCount = pagedResult.TotalPages ?? 0;
            PageNo = pageNo;
            PageSize = pageSize;

            foreach (var result in pagedResult.Items)
            {
                Add(result.ToEntity());
            }
        }

        public int TotalCount { get; }
        public int PageCount { get; }
        public int PageNo { get; }
        public int PageSize { get; }
    }
}