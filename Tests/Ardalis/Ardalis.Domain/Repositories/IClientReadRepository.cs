using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Domain.Entities;
using Ardalis.Specification;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Ardalis.Repositories.ReadRepositoryInterface", Version = "1.0")]

namespace Ardalis.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IClientReadRepository : IReadRepositoryBase<Client>
    {
        [IntentManaged(Mode.Fully)]
        Task<Client?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

        [IntentManaged(Mode.Fully)]
        Task<List<Client>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}