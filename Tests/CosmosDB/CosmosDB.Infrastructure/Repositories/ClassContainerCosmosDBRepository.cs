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
    internal class ClassContainerCosmosDBRepository : CosmosDBRepositoryBase<ClassContainer, ClassContainer, ClassContainerDocument>, IClassContainerRepository
    {
        public ClassContainerCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<ClassContainerDocument> cosmosRepository,
            IMapper mapper) : base(unitOfWork, cosmosRepository, mapper, "id")
        {
        }

        protected override void EnsureHasId(ClassContainer entity)
        {
            entity.Id ??= Guid.NewGuid().ToString();
        }
    }
}