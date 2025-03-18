using CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Entities;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Repositories;
using CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Persistence;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepository", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Infrastructure.Repositories
{
    public class ClientDaprStateStoreRepository : DaprStateStoreRepositoryBase<IClient>, IClientRepository
    {
        public ClientDaprStateStoreRepository(DaprClient daprClient, DaprStateStoreUnitOfWork unitOfWork) : base(daprClient: daprClient, unitOfWork: unitOfWork, enableTransactions: false, storeName: "statestore")
        {
        }

        public void Add(IClient entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Update(IClient entity)
        {
            Upsert(entity.Id, entity);
        }

        public void Remove(IClient entity)
        {
            Remove(entity.Id, entity);
        }

        public Task<IClient?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return FindByKeyAsync(id, cancellationToken);
        }

        public Task<List<IClient>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            return FindByKeysAsync(ids, cancellationToken);
        }
    }
}