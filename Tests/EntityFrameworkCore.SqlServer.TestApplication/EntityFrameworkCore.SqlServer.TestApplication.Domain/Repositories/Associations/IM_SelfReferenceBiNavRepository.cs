using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IM_SelfReferenceBiNavRepository : IEfRepository<M_SelfReferenceBiNav, M_SelfReferenceBiNav>
    {

        [IntentManaged(Mode.Fully)]
        Task<M_SelfReferenceBiNav> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<M_SelfReferenceBiNav>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}