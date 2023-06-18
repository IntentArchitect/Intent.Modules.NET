using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using CosmosRepository = Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal class ClientCosmosDBRepository : CosmosDBRepositoryBase<Client, Client, ClientDocument>, IClientRepository
    {
        public ClientCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<ClientDocument> cosmosRepository,
            IMapper mapper) : base(unitOfWork, cosmosRepository, mapper, "identifier")
        {
        }

        protected override void EnsureHasId(Client entity)
        {
            entity.Identifier ??= Guid.NewGuid().ToString();
        }
    }
}