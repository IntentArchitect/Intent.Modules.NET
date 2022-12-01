using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IImplicitKeyAggrRootRepository : IRepository<ImplicitKeyAggrRoot, ImplicitKeyAggrRoot>
    {

        [IntentManaged(Mode.Fully)]
        Task<ImplicitKeyAggrRoot> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ImplicitKeyAggrRoot>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}