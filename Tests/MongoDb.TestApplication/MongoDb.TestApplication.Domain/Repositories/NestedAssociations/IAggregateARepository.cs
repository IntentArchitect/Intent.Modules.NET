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
    public interface IAggregateARepository : IMongoRepository<AggregateA>
    {
        [IntentManaged(Mode.Fully)]
        List<AggregateA> SearchText(string searchText, Expression<Func<AggregateA, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(AggregateA entity);
        [IntentManaged(Mode.Fully)]
        Task<AggregateA?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateA>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}