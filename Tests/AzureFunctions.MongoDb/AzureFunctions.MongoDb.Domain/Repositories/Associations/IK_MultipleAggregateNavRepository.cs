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
    public interface IK_MultipleAggregateNavRepository : IMongoRepository<K_MultipleAggregateNav>
    {
        [IntentManaged(Mode.Fully)]
        List<K_MultipleAggregateNav> SearchText(string searchText, Expression<Func<K_MultipleAggregateNav, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(K_MultipleAggregateNav entity);
        [IntentManaged(Mode.Fully)]
        Task<K_MultipleAggregateNav?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<K_MultipleAggregateNav>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}