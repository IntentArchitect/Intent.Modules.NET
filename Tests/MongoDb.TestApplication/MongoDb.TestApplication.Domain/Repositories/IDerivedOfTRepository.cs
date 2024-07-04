using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories
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