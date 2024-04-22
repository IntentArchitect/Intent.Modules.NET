using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.PrimaryKeyTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INewClassLongRepository : IEFRepository<NewClassLong, NewClassLong>
    {
        [IntentManaged(Mode.Fully)]
        Task<NewClassLong?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NewClassLong>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}