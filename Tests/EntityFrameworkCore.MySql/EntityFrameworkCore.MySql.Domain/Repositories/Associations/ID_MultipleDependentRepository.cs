using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ID_MultipleDependentRepository : IEFRepository<D_MultipleDependent, D_MultipleDependent>
    {
        [IntentManaged(Mode.Fully)]
        Task<D_MultipleDependent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<D_MultipleDependent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}