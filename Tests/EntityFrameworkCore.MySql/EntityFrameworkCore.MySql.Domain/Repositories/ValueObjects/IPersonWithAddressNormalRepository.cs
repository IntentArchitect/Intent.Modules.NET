using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.ValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersonWithAddressNormalRepository : IEFRepository<PersonWithAddressNormal, PersonWithAddressNormal>
    {
        [IntentManaged(Mode.Fully)]
        Task<PersonWithAddressNormal?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PersonWithAddressNormal>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}