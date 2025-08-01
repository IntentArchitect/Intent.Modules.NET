using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.ToManyIds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IToManySourceRepository : IMongoRepository<ToManySource>
    {
        [IntentManaged(Mode.Fully)]
        List<ToManySource> SearchText(string searchText, Expression<Func<ToManySource, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(ToManySource entity);
        [IntentManaged(Mode.Fully)]
        Task<ToManySource?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ToManySource>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}