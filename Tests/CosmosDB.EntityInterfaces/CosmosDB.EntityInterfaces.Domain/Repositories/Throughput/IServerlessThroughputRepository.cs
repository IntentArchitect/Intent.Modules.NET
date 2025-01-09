using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Domain.Entities.Throughput;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Repositories.Throughput
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IServerlessThroughputRepository : ICosmosDBRepository<IServerlessThroughput, IServerlessThroughputDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<IServerlessThroughput?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}