using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IStandaloneDerivedRepository : IRepository<StandaloneDerived, StandaloneDerived>
    {

        [IntentManaged(Mode.Fully)]
        Task<StandaloneDerived> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<StandaloneDerived>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}