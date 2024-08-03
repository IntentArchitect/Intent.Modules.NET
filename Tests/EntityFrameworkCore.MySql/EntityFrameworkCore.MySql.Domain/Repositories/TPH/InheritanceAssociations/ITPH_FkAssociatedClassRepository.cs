using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_FkAssociatedClassRepository : IEFRepository<TPH_FkAssociatedClass, TPH_FkAssociatedClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_FkAssociatedClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_FkAssociatedClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}