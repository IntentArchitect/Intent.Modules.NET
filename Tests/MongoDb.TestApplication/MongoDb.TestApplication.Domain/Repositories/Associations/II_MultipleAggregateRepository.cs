using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface II_MultipleAggregateRepository : IRepository<I_MultipleAggregate, I_MultipleAggregate>
    {
        [IntentManaged(Mode.Fully)]
        Task<I_MultipleAggregate> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<I_MultipleAggregate>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}