using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.PrivateSetters.Domain.Entities.Throughput;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Throughput
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAutoscaleRepository : ICosmosDBRepository<Autoscale, IAutoscaleDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Autoscale?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}