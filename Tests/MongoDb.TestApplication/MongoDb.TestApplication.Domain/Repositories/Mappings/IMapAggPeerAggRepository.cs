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
    public interface IMapAggPeerAggRepository : IMongoRepository<MapAggPeerAgg>
    {
        [IntentManaged(Mode.Fully)]
        List<MapAggPeerAgg> SearchText(string searchText, Expression<Func<MapAggPeerAgg, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapAggPeerAgg entity);
        [IntentManaged(Mode.Fully)]
        Task<MapAggPeerAgg?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapAggPeerAgg>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}