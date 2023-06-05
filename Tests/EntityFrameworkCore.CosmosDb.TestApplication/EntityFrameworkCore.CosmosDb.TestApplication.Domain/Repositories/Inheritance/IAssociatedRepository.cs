using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.Inheritance
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAssociatedRepository : IEFRepository<Associated, Associated>
    {

        [IntentManaged(Mode.Fully)]
        Task<Associated> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Associated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}