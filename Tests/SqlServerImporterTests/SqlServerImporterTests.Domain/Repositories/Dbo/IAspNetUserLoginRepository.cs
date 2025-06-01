using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAspNetUserLoginRepository : IEFRepository<AspNetUserLogin, AspNetUserLogin>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((string LoginProvider, string ProviderKey) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserLogin?> FindByIdAsync((string LoginProvider, string ProviderKey) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserLogin?> FindByIdAsync((string LoginProvider, string ProviderKey) id, Func<IQueryable<AspNetUserLogin>, IQueryable<AspNetUserLogin>> queryOptions, CancellationToken cancellationToken = default);
    }
}