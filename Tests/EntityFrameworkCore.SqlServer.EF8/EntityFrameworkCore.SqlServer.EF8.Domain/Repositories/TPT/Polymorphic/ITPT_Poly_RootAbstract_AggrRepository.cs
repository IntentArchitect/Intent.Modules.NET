using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_Poly_RootAbstract_AggrRepository : IEFRepository<TPT_Poly_RootAbstract_Aggr, TPT_Poly_RootAbstract_Aggr>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_Poly_RootAbstract_Aggr?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_Poly_RootAbstract_Aggr>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}