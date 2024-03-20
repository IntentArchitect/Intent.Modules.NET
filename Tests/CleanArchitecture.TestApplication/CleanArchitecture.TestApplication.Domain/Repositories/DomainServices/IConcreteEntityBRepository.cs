using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Repositories.DomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IConcreteEntityBRepository : IEFRepository<ConcreteEntityB, ConcreteEntityB>
    {
        [IntentManaged(Mode.Fully)]
        Task<ConcreteEntityB?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ConcreteEntityB>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}