using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMapImplyOptionalRepository : IMongoRepository<MapImplyOptional>
    {
        [IntentManaged(Mode.Fully)]
        List<MapImplyOptional> SearchText(string searchText, Expression<Func<MapImplyOptional, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapImplyOptional entity);
        [IntentManaged(Mode.Fully)]
        Task<MapImplyOptional?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapImplyOptional>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}