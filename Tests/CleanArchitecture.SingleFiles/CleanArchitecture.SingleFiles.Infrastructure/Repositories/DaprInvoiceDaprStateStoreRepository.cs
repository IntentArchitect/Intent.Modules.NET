using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepository", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Repositories
{
    public class DaprInvoiceDaprStateStoreRepository : DaprStateStoreRepositoryBase<DaprInvoice>, IDaprInvoiceRepository
    {
        public DaprInvoiceDaprStateStoreRepository(DaprClient daprClient, DaprStateStoreUnitOfWork unitOfWork) : base(daprClient: daprClient, unitOfWork: unitOfWork, enableTransactions: false, storeName: "statestore")
        {
        }

        public void Add(DaprInvoice entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Update(DaprInvoice entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Remove(DaprInvoice entity)
        {
            Remove(entity.Id, entity);
        }

        public Task<DaprInvoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return FindByKeyAsync(id, cancellationToken);
        }

        public Task<List<DaprInvoice>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return FindByKeysAsync(ids, cancellationToken);
        }
    }
}