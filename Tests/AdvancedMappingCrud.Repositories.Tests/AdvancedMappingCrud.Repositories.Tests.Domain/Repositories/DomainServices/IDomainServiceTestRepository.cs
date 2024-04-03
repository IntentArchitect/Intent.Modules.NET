using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDomainServiceTestRepository : IEFRepository<DomainServiceTest, DomainServiceTest>
    {
        [IntentManaged(Mode.Fully)]
        Task<DomainServiceTest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DomainServiceTest>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}