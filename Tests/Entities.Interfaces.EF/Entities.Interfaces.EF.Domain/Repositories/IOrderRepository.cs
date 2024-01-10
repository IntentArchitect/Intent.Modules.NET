using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.Interfaces.EF.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.Interfaces.EF.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOrderRepository : IEFRepository<IOrder, Order>
    {
        [IntentManaged(Mode.Fully)]
        Task<IOrder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IOrder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}