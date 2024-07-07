using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Collections
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomCollectionEntityARepository : IMongoRepository<CustomCollectionEntityA>
    {
        [IntentManaged(Mode.Fully)]
        List<CustomCollectionEntityA> SearchText(string searchText, Expression<Func<CustomCollectionEntityA, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(CustomCollectionEntityA entity);
        [IntentManaged(Mode.Fully)]
        Task<CustomCollectionEntityA?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomCollectionEntityA>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}