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