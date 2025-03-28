using CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.DomainEntityInterfaces.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClientRepository : IDaprStateStoreRepository<IClient>
    {
        [IntentManaged(Mode.Fully)]
        Task<IClient?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IClient>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}