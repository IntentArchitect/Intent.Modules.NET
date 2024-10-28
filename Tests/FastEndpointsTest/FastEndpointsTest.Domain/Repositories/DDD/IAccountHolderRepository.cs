using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FastEndpointsTest.Domain.Repositories.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccountHolderRepository : IEFRepository<AccountHolder, AccountHolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccountHolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccountHolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}