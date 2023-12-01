using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Repositories.Mapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMappingRootRepository : IEFRepository<MappingRoot, MappingRoot>
    {
        [IntentManaged(Mode.Fully)]
        Task<MappingRoot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MappingRoot>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}