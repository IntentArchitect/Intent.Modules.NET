using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using MultipleDocumentStores.Domain.Repositories;
using MultipleDocumentStores.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosPagedList", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Repositories
{
    internal class CosmosPagedList<TDomain, TDocument> : List<TDomain>, IPagedList<TDomain>
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>
    {
        public CosmosPagedList(IEnumerable<TDomain> pagedResult, int totalCount, int pageCount, int pageNo, int pageSize)
        {
            TotalCount = totalCount;
            PageCount = pageCount;
            PageNo = pageNo;
            PageSize = pageSize;

            foreach (var result in pagedResult)
            {
                Add(result);
            }
        }

        public int TotalCount { get; }
        public int PageCount { get; }
        public int PageNo { get; }
        public int PageSize { get; }
    }
}