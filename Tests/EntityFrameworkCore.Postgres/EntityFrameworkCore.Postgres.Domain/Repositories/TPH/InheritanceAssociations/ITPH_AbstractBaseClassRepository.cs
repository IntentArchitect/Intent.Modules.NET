using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_AbstractBaseClassRepository : IEFRepository<TPH_AbstractBaseClass, TPH_AbstractBaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_AbstractBaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_AbstractBaseClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}