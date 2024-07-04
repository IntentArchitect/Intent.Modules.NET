using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.NestedAssociations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.NestedAssociations
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