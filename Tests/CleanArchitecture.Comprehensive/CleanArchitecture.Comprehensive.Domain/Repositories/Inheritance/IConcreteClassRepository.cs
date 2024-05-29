using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IConcreteClassRepository : IEFRepository<ConcreteClass, ConcreteClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<ConcreteClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ConcreteClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}