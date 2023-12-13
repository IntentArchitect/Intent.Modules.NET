using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IEntityOfTRepository<T> : ICosmosDBRepository<EntityOfT<T>, IEntityOfTDocument<T>>
    {
        [IntentManaged(Mode.Fully)]
        Task<EntityOfT<T>?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}