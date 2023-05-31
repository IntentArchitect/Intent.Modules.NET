using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Repositories.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITransactionRepository : IEfRepository<Transaction, Transaction>
    {
        [IntentManaged(Mode.Fully)]
        Task<Transaction> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Transaction>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}