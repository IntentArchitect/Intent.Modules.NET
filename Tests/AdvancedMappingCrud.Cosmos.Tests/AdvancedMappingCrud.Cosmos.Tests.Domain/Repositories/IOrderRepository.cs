using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOrderRepository : ICosmosDBRepository<Order, IOrderDocument>
    {
        Guid ExtendedSaveOrderOperation();
        [IntentManaged(Mode.Fully)]
        Task<Order?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}