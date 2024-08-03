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
    public interface ITPH_FkDerivedClassRepository : IEFRepository<TPH_FkDerivedClass, TPH_FkDerivedClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_FkDerivedClass?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, CancellationToken cancellationToken = default);
    }
}