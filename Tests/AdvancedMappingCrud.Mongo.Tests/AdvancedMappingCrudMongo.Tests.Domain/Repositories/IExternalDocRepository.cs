using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IExternalDocRepository : IMongoRepository<ExternalDoc>
    {
        [IntentManaged(Mode.Fully)]
        List<ExternalDoc> SearchText(string searchText, Expression<Func<ExternalDoc, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(ExternalDoc entity);
        [IntentManaged(Mode.Fully)]
        Task<ExternalDoc?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ExternalDoc>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}