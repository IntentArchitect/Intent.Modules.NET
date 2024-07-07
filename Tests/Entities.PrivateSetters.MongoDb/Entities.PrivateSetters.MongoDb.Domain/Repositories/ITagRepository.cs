using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITagRepository : IMongoRepository<Tag>
    {
        [IntentManaged(Mode.Fully)]
        List<Tag> SearchText(string searchText, Expression<Func<Tag, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(Tag entity);
        [IntentManaged(Mode.Fully)]
        Task<Tag?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Tag>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}