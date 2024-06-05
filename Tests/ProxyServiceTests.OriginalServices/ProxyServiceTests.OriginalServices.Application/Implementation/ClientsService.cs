using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Application.Clients;
using ProxyServiceTests.OriginalServices.Application.Interfaces;
using ProxyServiceTests.OriginalServices.Domain.Common.Exceptions;
using ProxyServiceTests.OriginalServices.Domain.Entities;
using ProxyServiceTests.OriginalServices.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Implementation
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
        public async Task<Guid> CreateClient(ClientCreateDto dto, CancellationToken cancellationToken = default)
        {
            var client = new Client
            {
                Name = dto.Name
            };

            _clientRepository.Add(client);
            await _clientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return client.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ClientDto> FindClientById(Guid id, CancellationToken cancellationToken = default)
        {
            var client = await _clientRepository.FindByIdAsync(id, cancellationToken);
            if (client is null)
            {
                throw new NotFoundException($"Could not find Client '{id}'");
            }
            return client.MapToClientDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClientDto>> FindClients(CancellationToken cancellationToken = default)
        {
            var clients = await _clientRepository.FindAllAsync(cancellationToken);
            return clients.MapToClientDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateClient(Guid id, ClientUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var client = await _clientRepository.FindByIdAsync(id, cancellationToken);
            if (client is null)
            {
                throw new NotFoundException($"Could not find Client '{id}'");
            }

            client.Name = dto.Name;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteClient(Guid id, CancellationToken cancellationToken = default)
        {
            var client = await _clientRepository.FindByIdAsync(id, cancellationToken);
            if (client is null)
            {
                throw new NotFoundException($"Could not find Client '{id}'");
            }

            _clientRepository.Remove(client);
        }

        public void Dispose()
        {
        }
    }
}