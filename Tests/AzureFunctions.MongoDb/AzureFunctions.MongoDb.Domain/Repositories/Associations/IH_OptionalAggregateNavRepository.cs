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
    public interface IH_OptionalAggregateNavRepository : IMongoRepository<H_OptionalAggregateNav>
    {
        [IntentManaged(Mode.Fully)]
        List<H_OptionalAggregateNav> SearchText(string searchText, Expression<Func<H_OptionalAggregateNav, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(H_OptionalAggregateNav entity);
        [IntentManaged(Mode.Fully)]
        Task<H_OptionalAggregateNav?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<H_OptionalAggregateNav>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}