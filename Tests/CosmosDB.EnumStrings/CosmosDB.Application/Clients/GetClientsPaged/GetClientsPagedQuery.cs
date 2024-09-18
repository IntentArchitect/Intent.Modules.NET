using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Clients.GetClientsPaged
{
    public class GetClientsPagedQuery : IRequest<PagedResult<ClientDto>>, IQuery
    {
        public GetClientsPagedQuery(int pageNo, int pageSize)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}