using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPoly_BaseClassNonAbstractRepository : IRepository<Poly_BaseClassNonAbstract, Poly_BaseClassNonAbstract>
    {

        [IntentManaged(Mode.Fully)]
        Task<Poly_BaseClassNonAbstract> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Poly_BaseClassNonAbstract>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}