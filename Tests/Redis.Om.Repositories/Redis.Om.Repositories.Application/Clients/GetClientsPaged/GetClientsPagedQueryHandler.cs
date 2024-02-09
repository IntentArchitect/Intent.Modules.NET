using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Pagination;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientsPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientsPagedQueryHandler : IRequestHandler<GetClientsPagedQuery, PagedResult<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetClientsPagedQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ClientDto>> Handle(GetClientsPagedQuery request, CancellationToken cancellationToken)
        {
            var clients = await _clientRepository.FindAllAsync(request.PageNo, request.PageSize, cancellationToken);
            return clients.MapToPagedResult(x => x.MapToClientDto(_mapper));
        }
    }
}