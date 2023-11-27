using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Controllers.Secured.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBuyerRepository : IEFRepository<Buyer, Buyer>
    {
        [IntentManaged(Mode.Fully)]
        Task<Buyer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Buyer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}