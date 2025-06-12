using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Clients;
using Standard.AspNetCore.TestApplication.Application.Common.Pagination;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Common.Exceptions;
using Standard.AspNetCore.TestApplication.Domain.Entities;
using Standard.AspNetCore.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
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
        public async Task<Guid> CreateClient(ClientCreate dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Clients.Client> FindClientById(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClientById (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<Clients.Client>> FindClients(CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClients (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<Clients.Client>> FindClientsPaged(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement FindClientsPaged (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateClient(Guid id, ClientUpdate dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteClient(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteClient (ClientsService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}