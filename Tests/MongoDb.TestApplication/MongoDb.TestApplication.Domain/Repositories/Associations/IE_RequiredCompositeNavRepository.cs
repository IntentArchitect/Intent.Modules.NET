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
    public interface IE_RequiredCompositeNavRepository : IMongoRepository<E_RequiredCompositeNav>
    {
        [IntentManaged(Mode.Fully)]
        List<E_RequiredCompositeNav> SearchText(string searchText, Expression<Func<E_RequiredCompositeNav, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(E_RequiredCompositeNav entity);
        [IntentManaged(Mode.Fully)]
        Task<E_RequiredCompositeNav?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<E_RequiredCompositeNav>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}