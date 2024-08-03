using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_Poly_ConcreteARepository : IEFRepository<TPT_Poly_ConcreteA, TPT_Poly_ConcreteA>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_ConcreteA?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_Poly_ConcreteA>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}