using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.EntityInterfaces.Application.Common.Pagination;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Clients.GetClientsPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientsPagedQueryHandler : IRequestHandler<GetClientsPagedQuery, PagedResult<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetClientsPagedQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ClientDto>> Handle(GetClientsPagedQuery request, CancellationToken cancellationToken)
        {
            var results = await _clientRepository.FindAllAsync(
                pageNo: request.PageNo,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            return results.MapToPagedResult(x => x.MapToClientDto(_mapper));
        }
    }
}