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
    public interface IMapperRootRepository : IMongoRepository<MapperRoot>
    {
        [IntentManaged(Mode.Fully)]
        List<MapperRoot> SearchText(string searchText, Expression<Func<MapperRoot, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapperRoot entity);
        [IntentManaged(Mode.Fully)]
        Task<MapperRoot?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapperRoot>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}