using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.MappingTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.MappingTests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INestingParentRepository : IEFRepository<NestingParent, NestingParent>
    {
        [IntentManaged(Mode.Fully)]
        Task<NestingParent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NestingParent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}