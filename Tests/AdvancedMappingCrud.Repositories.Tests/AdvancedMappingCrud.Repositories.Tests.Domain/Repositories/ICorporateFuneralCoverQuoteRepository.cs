using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICorporateFuneralCoverQuoteRepository : IEFRepository<CorporateFuneralCoverQuote, CorporateFuneralCoverQuote>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CorporateFuneralCoverQuote?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CorporateFuneralCoverQuote?> FindByIdAsync(Guid id, Func<IQueryable<CorporateFuneralCoverQuote>, IQueryable<CorporateFuneralCoverQuote>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CorporateFuneralCoverQuote>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}