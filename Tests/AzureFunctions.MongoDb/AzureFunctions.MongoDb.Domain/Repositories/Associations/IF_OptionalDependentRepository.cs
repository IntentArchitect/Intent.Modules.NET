using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IF_OptionalDependentRepository : IMongoRepository<F_OptionalDependent>
    {
        [IntentManaged(Mode.Fully)]
        List<F_OptionalDependent> SearchText(string searchText, Expression<Func<F_OptionalDependent, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(F_OptionalDependent entity);
        [IntentManaged(Mode.Fully)]
        Task<F_OptionalDependent?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<F_OptionalDependent>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}