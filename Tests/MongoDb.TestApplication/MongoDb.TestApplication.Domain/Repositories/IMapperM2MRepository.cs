using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMapperM2MRepository : IMongoRepository<MapperM2M>
    {
        [IntentManaged(Mode.Fully)]
        List<MapperM2M> SearchText(string searchText, Expression<Func<MapperM2M, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MapperM2M entity);
        [IntentManaged(Mode.Fully)]
        Task<MapperM2M?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MapperM2M>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}