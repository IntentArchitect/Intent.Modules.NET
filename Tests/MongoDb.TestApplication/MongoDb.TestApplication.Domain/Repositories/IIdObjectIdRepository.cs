using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface IIdObjectIdRepository : IRepository<IdObjectId, IdObjectId>
    {

        [IntentManaged(Mode.Fully)]
        object Update(Expression<Func<IdObjectId, bool>> predicate, IdObjectId entity);
        [IntentManaged(Mode.Fully)]
        Task<IdObjectId> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdObjectId>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}