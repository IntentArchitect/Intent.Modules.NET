using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersonWithAddressNormalRepository : IEFRepository<PersonWithAddressNormal, PersonWithAddressNormal>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressNormal?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressNormal?> FindByIdAsync(Guid id, Func<IQueryable<PersonWithAddressNormal>, IQueryable<PersonWithAddressNormal>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PersonWithAddressNormal>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}