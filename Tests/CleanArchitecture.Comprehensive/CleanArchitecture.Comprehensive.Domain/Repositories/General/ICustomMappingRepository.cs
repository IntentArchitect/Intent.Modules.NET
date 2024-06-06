using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.General;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.General
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomMappingRepository : IEFRepository<CustomMapping, CustomMapping>
    {
        [IntentManaged(Mode.Fully)]
        Task<CustomMapping?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomMapping>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}