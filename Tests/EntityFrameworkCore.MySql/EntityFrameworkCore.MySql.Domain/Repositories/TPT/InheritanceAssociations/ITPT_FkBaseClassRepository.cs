using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_FkBaseClassRepository : IEFRepository<TPT_FkBaseClass, TPT_FkBaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_FkBaseClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPT_FkBaseClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, Func<IQueryable<TPT_FkBaseClass>, IQueryable<TPT_FkBaseClass>> queryOptions, CancellationToken cancellationToken = default);
    }
}