using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_ConcreteBaseClassRepository : IEFRepository<TPT_ConcreteBaseClass, TPT_ConcreteBaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_ConcreteBaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_ConcreteBaseClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}