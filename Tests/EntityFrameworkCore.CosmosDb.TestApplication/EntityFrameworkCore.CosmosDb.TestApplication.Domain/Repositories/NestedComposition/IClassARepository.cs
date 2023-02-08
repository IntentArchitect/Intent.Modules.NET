using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.NestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClassARepository : IRepository<ClassA, ClassA>
    {

        [IntentManaged(Mode.Fully)]
        Task<ClassA> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ClassA>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}