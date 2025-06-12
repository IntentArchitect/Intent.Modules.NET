using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Collections;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Collections
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomCollectionEntityBRepository : IMongoRepository<CustomCollectionEntityB>
    {
        [IntentManaged(Mode.Fully)]
        List<CustomCollectionEntityB> SearchText(string searchText, Expression<Func<CustomCollectionEntityB, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(CustomCollectionEntityB entity);
        [IntentManaged(Mode.Fully)]
        Task<CustomCollectionEntityB?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomCollectionEntityB>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}