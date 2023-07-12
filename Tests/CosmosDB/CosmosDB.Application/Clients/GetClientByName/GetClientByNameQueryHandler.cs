using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Clients.GetClientByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientByNameQueryHandler : IRequestHandler<GetClientByNameQuery, List<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetClientByNameQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ClientDto>> Handle(GetClientByNameQuery request, CancellationToken cancellationToken)
        {
            var clients = await _clientRepository.FindAllAsync(c => c.Name.Contains(request.SearchText), cancellationToken);
            return clients.MapToClientDtoList(_mapper);
        }
    }
}