using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.ComplexTypes;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.ComplexTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOrderCTRepository : IEFRepository<OrderCT, OrderCT>
    {
        [IntentManaged(Mode.Fully)]
        Task<OrderCT?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OrderCT>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}