using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.IdTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IIdTypeGuidRepository : IMongoRepository<IdTypeGuid>
    {
        [IntentManaged(Mode.Fully)]
        List<IdTypeGuid> SearchText(string searchText, Expression<Func<IdTypeGuid, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(IdTypeGuid entity);
        [IntentManaged(Mode.Fully)]
        Task<IdTypeGuid?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdTypeGuid>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}