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
    public interface ITextIndexEntityRepository : IMongoRepository<TextIndexEntity>
    {
        [IntentManaged(Mode.Fully)]
        List<TextIndexEntity> SearchText(string searchText, Expression<Func<TextIndexEntity, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(TextIndexEntity entity);
        [IntentManaged(Mode.Fully)]
        Task<TextIndexEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TextIndexEntity>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}