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
    public interface IA_Aggregation_SingleRepository : IRepository<IA_Aggregation_Single, A_Aggregation_Single>
    {
        [IntentManaged(Mode.Fully)]
        Task<IA_Aggregation_Single> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IA_Aggregation_Single>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}