using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Repositories.CRUD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IImplicitKeyAggrRootRepository : IEfRepository<ImplicitKeyAggrRoot, ImplicitKeyAggrRoot>
    {

        [IntentManaged(Mode.Fully)]
        Task<ImplicitKeyAggrRoot> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ImplicitKeyAggrRoot>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}