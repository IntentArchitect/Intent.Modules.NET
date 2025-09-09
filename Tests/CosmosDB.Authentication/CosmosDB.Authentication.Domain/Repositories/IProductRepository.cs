using CosmosDB.Authentication.Domain.Entities;
using CosmosDB.Authentication.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.Authentication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IProductRepository : ICosmosDBRepository<Product, IProductDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Product?> FindByIdAsync((string Id, string Name) id, CancellationToken cancellationToken = default);
    }
}