using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClassicDomainServiceTestRepository : IEFRepository<ClassicDomainServiceTest, ClassicDomainServiceTest>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ClassicDomainServiceTest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ClassicDomainServiceTest?> FindByIdAsync(Guid id, Func<IQueryable<ClassicDomainServiceTest>, IQueryable<ClassicDomainServiceTest>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ClassicDomainServiceTest>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}