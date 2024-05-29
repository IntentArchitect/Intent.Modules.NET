using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.DefaultDiagram;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.DefaultDiagram
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClassWithDefaultRepository : IEFRepository<ClassWithDefault, ClassWithDefault>
    {
        [IntentManaged(Mode.Fully)]
        Task<ClassWithDefault?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ClassWithDefault>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}