using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ID_OptionalAggregateRepository : IMongoRepository<D_OptionalAggregate>
    {
        [IntentManaged(Mode.Fully)]
        List<D_OptionalAggregate> SearchText(string searchText, Expression<Func<D_OptionalAggregate, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(D_OptionalAggregate entity);
        [IntentManaged(Mode.Fully)]
        Task<D_OptionalAggregate?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<D_OptionalAggregate>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}