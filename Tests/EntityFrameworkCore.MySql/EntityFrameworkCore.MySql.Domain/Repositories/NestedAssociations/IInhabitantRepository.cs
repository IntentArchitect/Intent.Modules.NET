using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.NestedAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IInhabitantRepository : IEFRepository<Inhabitant, Inhabitant>
    {
        [IntentManaged(Mode.Fully)]
        Task<Inhabitant?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Inhabitant>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}