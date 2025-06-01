using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.CustomPkName;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.CustomPkName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomPkCompRepository : IEFRepository<CustomPkComp, CustomPkComp>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((Guid MyId, string MyId2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CustomPkComp?> FindByIdAsync((Guid MyId, string MyId2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CustomPkComp?> FindByIdAsync((Guid MyId, string MyId2) id, Func<IQueryable<CustomPkComp>, IQueryable<CustomPkComp>> queryOptions, CancellationToken cancellationToken = default);
    }
}