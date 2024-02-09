using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Common.Exceptions;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientsByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientsByNameQueryHandler : IRequestHandler<GetClientsByNameQuery, List<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetClientsByNameQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> Handle(GetClientsByNameQuery request, CancellationToken cancellationToken)
        {
            var entity = await _clientRepository.FindAllAsync(x => x.Name == request.Name, cancellationToken);
            return entity.MapToClientDtoList(_mapper);
        }
    }
}