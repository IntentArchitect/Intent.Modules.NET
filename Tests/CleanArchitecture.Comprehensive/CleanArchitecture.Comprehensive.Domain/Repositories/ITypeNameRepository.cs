using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITypeNameRepository : IEFRepository<Entities.TypeName, Entities.TypeName>
    {
        [IntentManaged(Mode.Fully)]
        Task<Entities.TypeName?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Entities.TypeName>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}