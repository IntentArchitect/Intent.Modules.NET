using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.ToManyIds;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.ToManyIds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IToManySourceRepository : IMongoRepository<ToManySource, string>
    {
        [IntentManaged(Mode.Fully)]
        Task<ToManySource?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ToManySource>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}