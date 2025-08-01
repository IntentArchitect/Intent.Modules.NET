using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateBRepository : IMongoRepository<AggregateB>
    {
        [IntentManaged(Mode.Fully)]
        List<AggregateB> SearchText(string searchText, Expression<Func<AggregateB, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(AggregateB entity);
        [IntentManaged(Mode.Fully)]
        Task<AggregateB?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateB>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}