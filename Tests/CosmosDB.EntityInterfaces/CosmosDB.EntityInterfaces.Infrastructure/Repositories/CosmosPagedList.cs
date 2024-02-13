using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosPagedList", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories
{
    internal class CosmosPagedList<TDomain, TDomainState, TDocument> : List<TDomain>, IPagedList<TDomain>
        where TDomain : class
        where TDomainState : class, TDomain
        where TDocument : ICosmosDBDocument<TDomain, TDomainState, TDocument>
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