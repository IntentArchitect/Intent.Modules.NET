using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ExtensiveDomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBaseEntityBRepository : IEFRepository<BaseEntityB, BaseEntityB>
    {
        [IntentManaged(Mode.Fully)]
        Task<BaseEntityB?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<BaseEntityB>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}