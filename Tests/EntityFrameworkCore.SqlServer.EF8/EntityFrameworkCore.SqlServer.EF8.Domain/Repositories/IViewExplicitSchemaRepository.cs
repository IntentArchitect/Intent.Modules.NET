using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IViewExplicitSchemaRepository : IEFRepository<ViewExplicitSchema, ViewExplicitSchema>
    {
        [IntentManaged(Mode.Fully)]
        Task<ViewExplicitSchema?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ViewExplicitSchema>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}