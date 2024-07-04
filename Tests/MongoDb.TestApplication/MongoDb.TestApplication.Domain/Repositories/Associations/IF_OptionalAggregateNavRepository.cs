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
    public interface IF_OptionalAggregateNavRepository : IMongoRepository<F_OptionalAggregateNav>
    {
        [IntentManaged(Mode.Fully)]
        List<F_OptionalAggregateNav> SearchText(string searchText, Expression<Func<F_OptionalAggregateNav, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(F_OptionalAggregateNav entity);
        [IntentManaged(Mode.Fully)]
        Task<F_OptionalAggregateNav?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<F_OptionalAggregateNav>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}