using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities.Throughput;
using CosmosDB.Domain.Repositories.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Throughput
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IManualRepository : ICosmosDBRepository<Manual, IManualDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Manual?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}