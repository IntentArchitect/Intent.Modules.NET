using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPC.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPC.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_Poly_SecondLevelRepository : IEFRepository<TPC_Poly_SecondLevel, TPC_Poly_SecondLevel>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_Poly_SecondLevel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_Poly_SecondLevel>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}