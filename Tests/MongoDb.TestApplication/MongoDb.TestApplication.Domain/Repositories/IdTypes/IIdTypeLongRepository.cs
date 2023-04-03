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
    public interface IIdTypeLongRepository : IRepository<IdTypeLong, IdTypeLong>
    {

        [IntentManaged(Mode.Fully)]
        List<IdTypeLong> SearchText(string searchText, Expression<Func<IdTypeLong, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(IdTypeLong entity);
        [IntentManaged(Mode.Fully)]
        Task<IdTypeLong> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdTypeLong>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}