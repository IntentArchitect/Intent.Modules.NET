using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDerivedTypeRepository : ICosmosDBRepository<DerivedType, IDerivedTypeDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<DerivedType?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}