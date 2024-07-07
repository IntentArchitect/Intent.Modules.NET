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
    public interface IB_OptionalAggregateRepository : IMongoRepository<B_OptionalAggregate>
    {
        [IntentManaged(Mode.Fully)]
        List<B_OptionalAggregate> SearchText(string searchText, Expression<Func<B_OptionalAggregate, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(B_OptionalAggregate entity);
        [IntentManaged(Mode.Fully)]
        Task<B_OptionalAggregate?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<B_OptionalAggregate>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}