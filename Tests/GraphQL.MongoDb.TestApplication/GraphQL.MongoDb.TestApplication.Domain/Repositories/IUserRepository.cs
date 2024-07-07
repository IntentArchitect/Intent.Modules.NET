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
    public interface IUserRepository : IMongoRepository<User>
    {
        [IntentManaged(Mode.Fully)]
        List<User> SearchText(string searchText, Expression<Func<User, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(User entity);
        [IntentManaged(Mode.Fully)]
        Task<User?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<User>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}