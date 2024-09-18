using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Clients.GetClients
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, List<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _clientRepository.FindAllAsync(cancellationToken);
            return clients.MapToClientDtoList(_mapper);
        }
    }
}