using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDerivedRepository : IMongoRepository<Derived>
    {
        [IntentManaged(Mode.Fully)]
        List<Derived> SearchText(string searchText, Expression<Func<Derived, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(Derived entity);
        [IntentManaged(Mode.Fully)]
        Task<Derived?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Derived>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}