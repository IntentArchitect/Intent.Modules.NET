using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.IdTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.IdTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IIdTypeOjectIdStrRepository : IMongoRepository<IdTypeOjectIdStr, string>
    {
        [IntentManaged(Mode.Fully)]
        Task<IdTypeOjectIdStr?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IdTypeOjectIdStr>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}