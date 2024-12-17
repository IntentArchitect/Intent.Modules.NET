using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Application.Clients;
using CosmosDB.Application.Interfaces.ServiceTesting;
using CosmosDB.Domain;
using CosmosDB.Domain.Repositories;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CosmosDB.Application.Implementation.ServiceTesting
{
    [IntentManaged(Mode.Merge)]
    public class ClientsService : IClientsService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public ClientsService(IClientRepository clientRepository, IMapper mapper)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> GetClientsFilteredQuery(
            ClientType? type,
            string? name,
            CancellationToken cancellationToken = default)
        {
            IQueryable<IClientDocument> FilterClients(IQueryable<IClientDocument> queryable)
            {
                if (type != null)
                {
                    queryable = queryable.Where(x => x.Type == type);
                }

                if (name != null)
                {
                    queryable = queryable.Where(x => x.Name == name);
                }
                return queryable;
            }

            var entity = await _clientRepository.FindAllAsync(FilterClients, cancellationToken);
            return entity.MapToClientDtoList(_mapper);
        }
    }
}