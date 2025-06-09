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
    public interface IA_RequiredCompositeRepository : IMongoRepository<A_RequiredComposite>
    {
        [IntentManaged(Mode.Fully)]
        List<A_RequiredComposite> SearchText(string searchText, Expression<Func<A_RequiredComposite, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(A_RequiredComposite entity);
        [IntentManaged(Mode.Fully)]
        Task<A_RequiredComposite?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<A_RequiredComposite>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}