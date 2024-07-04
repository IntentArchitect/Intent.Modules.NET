using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMapAggChildRepository : IMongoRepository<MapAggChild>
    {
        [IntentManaged(Mode.Fully)]
        List<MapAggChild> SearchText(string searchText, Expression<Func<MapAggChild, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapAggChild entity);
        [IntentManaged(Mode.Fully)]
        Task<MapAggChild?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapAggChild>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}