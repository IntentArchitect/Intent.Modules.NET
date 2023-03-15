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
    public interface IIdIntRepository : IRepository<IdInt, IdInt>
    {

        [IntentManaged(Mode.Fully)]
        object Update(Expression<Func<IdInt, bool>> predicate, IdInt entity);
        [IntentManaged(Mode.Fully)]
        Task<IdInt> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdInt>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}