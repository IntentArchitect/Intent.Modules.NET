using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICompoundIndexEntityRepository : IMongoRepository<CompoundIndexEntity>
    {
        [IntentManaged(Mode.Fully)]
        List<CompoundIndexEntity> SearchText(string searchText, Expression<Func<CompoundIndexEntity, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(CompoundIndexEntity entity);
        [IntentManaged(Mode.Fully)]
        Task<CompoundIndexEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CompoundIndexEntity>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}