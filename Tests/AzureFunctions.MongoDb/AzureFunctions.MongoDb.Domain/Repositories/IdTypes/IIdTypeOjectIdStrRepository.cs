using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.IdTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IIdTypeOjectIdStrRepository : IMongoRepository<IdTypeOjectIdStr>
    {
        [IntentManaged(Mode.Fully)]
        List<IdTypeOjectIdStr> SearchText(string searchText, Expression<Func<IdTypeOjectIdStr, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(IdTypeOjectIdStr entity);
        [IntentManaged(Mode.Fully)]
        Task<IdTypeOjectIdStr?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdTypeOjectIdStr>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}