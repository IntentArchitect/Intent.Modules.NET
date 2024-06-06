using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Indexing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.Indexing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFilteredIndexRepository : IEFRepository<FilteredIndex, FilteredIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<FilteredIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FilteredIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}