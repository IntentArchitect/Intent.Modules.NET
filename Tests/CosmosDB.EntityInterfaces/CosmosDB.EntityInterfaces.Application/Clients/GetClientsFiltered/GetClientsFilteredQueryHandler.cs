using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Clients.GetClientsFiltered
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetClientsFilteredQueryHandler : IRequestHandler<GetClientsFilteredQuery, List<ClientDto>>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetClientsFilteredQueryHandler(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> Handle(GetClientsFilteredQuery request, CancellationToken cancellationToken)
        {
            IQueryable<IClientDocument> FilterClients(IQueryable<IClientDocument> queryable)
            {
                if (request.Type != null)
                {
                    queryable = queryable.Where(x => x.Type == request.Type);
                }

                if (request.Name != null)
                {
                    queryable = queryable.Where(x => x.Name == request.Name);
                }
                return queryable;
            }

            var entity = await _clientRepository.FindAllAsync(FilterClients, cancellationToken);
            return entity.MapToClientDtoList(_mapper);
        }
    }
}