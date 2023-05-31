using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITagRepository : IEfRepository<Tag, Tag>
    {
        [IntentManaged(Mode.Fully)]
        Task<Tag> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Tag>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}