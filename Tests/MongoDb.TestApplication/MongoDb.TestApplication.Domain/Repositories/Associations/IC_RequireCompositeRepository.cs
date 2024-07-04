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
    public interface IC_RequireCompositeRepository : IMongoRepository<C_RequireComposite>
    {
        [IntentManaged(Mode.Fully)]
        List<C_RequireComposite> SearchText(string searchText, Expression<Func<C_RequireComposite, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(C_RequireComposite entity);
        [IntentManaged(Mode.Fully)]
        Task<C_RequireComposite?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<C_RequireComposite>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}