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
    public interface IGiftCardRepository : IEFRepository<GiftCard, GiftCard>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(string cardCode, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<GiftCard?> FindByIdAsync(string cardCode, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<GiftCard?> FindByIdAsync(string cardCode, Func<IQueryable<GiftCard>, IQueryable<GiftCard>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<GiftCard>> FindByIdsAsync(string[] cardCodes, CancellationToken cancellationToken = default);
    }
}