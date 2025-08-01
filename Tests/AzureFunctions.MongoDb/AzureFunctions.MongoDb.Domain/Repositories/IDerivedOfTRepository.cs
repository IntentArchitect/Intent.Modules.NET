using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDerivedOfTRepository : IMongoRepository<DerivedOfT>
    {
        [IntentManaged(Mode.Fully)]
        List<DerivedOfT> SearchText(string searchText, Expression<Func<DerivedOfT, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(DerivedOfT entity);
        [IntentManaged(Mode.Fully)]
        Task<DerivedOfT?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DerivedOfT>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}