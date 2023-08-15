using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Entities;
using MultipleDocumentStores.Domain.Repositories;
using MultipleDocumentStores.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepository", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Repositories
{
    public class CustomerDaprDaprStateStoreRepository : DaprStateStoreRepositoryBase<CustomerDapr>, ICustomerDaprRepository
    {
        public CustomerDaprDaprStateStoreRepository(DaprClient daprClient, DaprStateStoreUnitOfWork unitOfWork) : base(daprClient: daprClient, unitOfWork: unitOfWork, enableTransactions: false, storeName: "statestore")
        {
        }

        public void Add(CustomerDapr entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Update(CustomerDapr entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Remove(CustomerDapr entity)
        {
            Remove(entity.Id, entity);
        }

        public Task<CustomerDapr?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return FindByKeyAsync(id, cancellationToken);
        }

        public Task<List<CustomerDapr>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return FindByKeysAsync(ids, cancellationToken);
        }
    }
}