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
    public interface IIdTypeIntRepository : IRepository<IdTypeInt, IdTypeInt>
    {

        [IntentManaged(Mode.Fully)]
        List<IdTypeInt> SearchText(string searchText, Expression<Func<IdTypeInt, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(IdTypeInt entity);
        [IntentManaged(Mode.Fully)]
        Task<IdTypeInt> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdTypeInt>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}