using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IB_OptionalDependentRepository : IMongoRepository<B_OptionalDependent>
    {
        [IntentManaged(Mode.Fully)]
        List<B_OptionalDependent> SearchText(string searchText, Expression<Func<B_OptionalDependent, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(B_OptionalDependent entity);
        [IntentManaged(Mode.Fully)]
        Task<B_OptionalDependent?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<B_OptionalDependent>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}