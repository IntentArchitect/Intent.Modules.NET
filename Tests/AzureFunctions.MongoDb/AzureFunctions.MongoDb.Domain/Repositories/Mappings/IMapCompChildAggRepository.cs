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
    public interface IMapCompChildAggRepository : IMongoRepository<MapCompChildAgg>
    {
        [IntentManaged(Mode.Fully)]
        List<MapCompChildAgg> SearchText(string searchText, Expression<Func<MapCompChildAgg, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapCompChildAgg entity);
        [IntentManaged(Mode.Fully)]
        Task<MapCompChildAgg?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapCompChildAgg>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}