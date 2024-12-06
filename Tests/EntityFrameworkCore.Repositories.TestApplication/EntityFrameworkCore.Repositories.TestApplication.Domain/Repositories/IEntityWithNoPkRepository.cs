using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IEntityWithNoPkRepository : IEFRepository<EntityWithNoPk, EntityWithNoPk>
    {
        Task StoredProcedure(CancellationToken cancellationToken = default);
    }
}