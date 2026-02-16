using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients.GetClientExtraFields
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientExtraFieldsQueryHandler : IRequestHandler<GetClientExtraFieldsQuery, List<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetClientExtraFieldsQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> Handle(GetClientExtraFieldsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _clientRepository.FindAllAsync(cancellationToken);
            return clients.MapToClientDtoList(_mapper);
        }
    }
}