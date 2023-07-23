using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using CleanArchitecture.Dapr.Infrastructure.Persistence;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepository", Version = "1.0")]

namespace CleanArchitecture.Dapr.Infrastructure.Repositories
{
    public class RegionDaprStateStoreRepository : DaprStateStoreRepositoryBase<Region>, IRegionRepository
    {
        public RegionDaprStateStoreRepository(DaprClient daprClient, DaprStateStoreUnitOfWork unitOfWork) : base(daprClient: daprClient, unitOfWork: unitOfWork, enableTransactions: false, storeName: "statestore")
        {
        }

        public void Add(Region entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Update(Region entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Remove(Region entity)
        {
            Remove(entity.Id, entity);
        }

        public Task<Region?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return FindByKeyAsync(id, cancellationToken);
        }

        public Task<List<Region>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return FindByKeysAsync(ids, cancellationToken);
        }
    }
}