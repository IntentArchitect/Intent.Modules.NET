using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EnumStrings.Domain.Entities;
using CosmosDB.EnumStrings.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.EnumStrings.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IRootEntityRepository : ICosmosDBRepository<RootEntity, IRootEntityDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<RootEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}