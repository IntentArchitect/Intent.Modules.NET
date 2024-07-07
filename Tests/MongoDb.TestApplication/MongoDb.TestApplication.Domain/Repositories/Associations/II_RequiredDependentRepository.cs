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
    public interface II_RequiredDependentRepository : IMongoRepository<I_RequiredDependent>
    {
        [IntentManaged(Mode.Fully)]
        List<I_RequiredDependent> SearchText(string searchText, Expression<Func<I_RequiredDependent, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(I_RequiredDependent entity);
        [IntentManaged(Mode.Fully)]
        Task<I_RequiredDependent?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<I_RequiredDependent>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}