using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOneToOneSourceRepository : IEFRepository<OneToOneSource, OneToOneSource>
    {
        [IntentManaged(Mode.Fully)]
        Task<OneToOneSource?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OneToOneSource>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}