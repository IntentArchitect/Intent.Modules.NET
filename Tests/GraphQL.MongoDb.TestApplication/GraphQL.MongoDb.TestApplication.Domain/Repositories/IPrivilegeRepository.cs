using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPrivilegeRepository : IMongoRepository<Privilege>
    {
        [IntentManaged(Mode.Fully)]
        List<Privilege> SearchText(string searchText, Expression<Func<Privilege, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(Privilege entity);
        [IntentManaged(Mode.Fully)]
        Task<Privilege?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Privilege>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}