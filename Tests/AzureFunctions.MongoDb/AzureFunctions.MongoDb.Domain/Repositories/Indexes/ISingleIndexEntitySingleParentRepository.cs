using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ISingleIndexEntitySingleParentRepository : IMongoRepository<SingleIndexEntitySingleParent>
    {
        [IntentManaged(Mode.Fully)]
        List<SingleIndexEntitySingleParent> SearchText(string searchText, Expression<Func<SingleIndexEntitySingleParent, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(SingleIndexEntitySingleParent entity);
        [IntentManaged(Mode.Fully)]
        Task<SingleIndexEntitySingleParent?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<SingleIndexEntitySingleParent>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}