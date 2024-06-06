using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IODataAggRepository : IEFRepository<ODataAgg, ODataAgg>
    {
        [IntentManaged(Mode.Fully)]
        Task<ODataAgg?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ODataAgg>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}