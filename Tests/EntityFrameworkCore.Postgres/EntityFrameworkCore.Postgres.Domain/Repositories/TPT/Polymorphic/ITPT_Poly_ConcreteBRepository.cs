using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_Poly_ConcreteBRepository : IEFRepository<TPT_Poly_ConcreteB, TPT_Poly_ConcreteB>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_ConcreteB?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_Poly_ConcreteB>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}