using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.CRUD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateSingleCRepository : IEFRepository<AggregateSingleC, AggregateSingleC>
    {

        [IntentManaged(Mode.Fully)]
        Task<AggregateSingleC?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateSingleC>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}