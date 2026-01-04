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
    public interface ICategoryRepository : ICosmosDBRepository<Category, ICategoryDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Category?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Category>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}