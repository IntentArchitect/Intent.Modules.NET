using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersonWithAddressSerializedRepository : IEFRepository<PersonWithAddressSerialized, PersonWithAddressSerialized>
    {
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressSerialized?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressSerialized?> FindByIdAsync(Guid id, Func<IQueryable<PersonWithAddressSerialized>, IQueryable<PersonWithAddressSerialized>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PersonWithAddressSerialized>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}