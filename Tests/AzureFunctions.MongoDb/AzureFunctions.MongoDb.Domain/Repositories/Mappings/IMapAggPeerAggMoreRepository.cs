using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMapAggPeerAggMoreRepository : IMongoRepository<MapAggPeerAggMore>
    {
        [IntentManaged(Mode.Fully)]
        List<MapAggPeerAggMore> SearchText(string searchText, Expression<Func<MapAggPeerAggMore, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapAggPeerAggMore entity);
        [IntentManaged(Mode.Fully)]
        Task<MapAggPeerAggMore?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapAggPeerAggMore>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}