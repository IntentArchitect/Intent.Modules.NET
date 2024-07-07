using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ISingleIndexEntityRepository : IMongoRepository<SingleIndexEntity>
    {
        [IntentManaged(Mode.Fully)]
        List<SingleIndexEntity> SearchText(string searchText, Expression<Func<SingleIndexEntity, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(SingleIndexEntity entity);
        [IntentManaged(Mode.Fully)]
        Task<SingleIndexEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<SingleIndexEntity>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}