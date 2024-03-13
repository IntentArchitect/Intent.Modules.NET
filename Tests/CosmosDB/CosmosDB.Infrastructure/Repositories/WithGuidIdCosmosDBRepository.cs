using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using CosmosDB.Domain.Repositories.Documents;
using CosmosDB.Infrastructure.Persistence;
using CosmosDB.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace CosmosDB.Infrastructure.Repositories
{
    internal class WithGuidIdCosmosDBRepository : CosmosDBRepositoryBase<WithGuidId, WithGuidIdDocument, IWithGuidIdDocument>, IWithGuidIdRepository
    {
        public WithGuidIdCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<WithGuidIdDocument> cosmosRepository,
            ICurrentUserService currentUserService) : base(unitOfWork, cosmosRepository, "id", currentUserService)
        {
        }

        public async Task<WithGuidId?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<WithGuidId>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}