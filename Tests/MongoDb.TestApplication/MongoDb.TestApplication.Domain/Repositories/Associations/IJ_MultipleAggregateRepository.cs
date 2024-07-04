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
    public interface IJ_MultipleAggregateRepository : IMongoRepository<J_MultipleAggregate>
    {
        [IntentManaged(Mode.Fully)]
        List<J_MultipleAggregate> SearchText(string searchText, Expression<Func<J_MultipleAggregate, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(J_MultipleAggregate entity);
        [IntentManaged(Mode.Fully)]
        Task<J_MultipleAggregate?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<J_MultipleAggregate>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}