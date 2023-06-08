using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDictionaryWithKvPSerializedRepository : IEFRepository<DictionaryWithKvPSerialized, DictionaryWithKvPSerialized>
    {
        [IntentManaged(Mode.Fully)]
        Task<DictionaryWithKvPSerialized?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DictionaryWithKvPSerialized>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}