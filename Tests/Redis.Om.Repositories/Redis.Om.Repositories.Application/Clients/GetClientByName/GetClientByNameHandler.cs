using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Common.Exceptions;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.GetClientByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientByNameHandler : IRequestHandler<GetClientByName, ClientDto>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetClientByNameHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ClientDto> Handle(GetClientByName request, CancellationToken cancellationToken)
        {
            var entity = await _clientRepository.FindAsync(x => x.Name == request.Name, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Client '{request.Name}'");
            }
            return entity.MapToClientDto(_mapper);
        }
    }
}