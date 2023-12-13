using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IEntityOfTRepository<T> : ICosmosDBRepository<IEntityOfT<T>, IEntityOfTDocument<T>>
    {
        [IntentManaged(Mode.Fully)]
        Task<IEntityOfT<T>?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}