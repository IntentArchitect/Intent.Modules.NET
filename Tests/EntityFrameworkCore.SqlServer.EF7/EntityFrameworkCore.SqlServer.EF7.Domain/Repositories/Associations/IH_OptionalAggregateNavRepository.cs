using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IH_OptionalAggregateNavRepository : IEFRepository<H_OptionalAggregateNav, H_OptionalAggregateNav>
    {
        [IntentManaged(Mode.Fully)]
        Task<H_OptionalAggregateNav?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<H_OptionalAggregateNav>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}