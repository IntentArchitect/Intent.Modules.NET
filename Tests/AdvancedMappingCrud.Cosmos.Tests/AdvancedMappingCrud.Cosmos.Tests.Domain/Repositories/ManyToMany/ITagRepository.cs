using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.ManyToMany
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITagRepository : ICosmosDBRepository<Tag, ITagDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Tag?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Tag>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}