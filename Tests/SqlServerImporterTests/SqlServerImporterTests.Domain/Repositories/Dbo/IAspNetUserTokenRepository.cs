using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlServerImporterTests.Domain.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAspNetUserTokenRepository : IEFRepository<AspNetUserToken, AspNetUserToken>
    {
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserToken?> FindByIdAsync((string UserId, string LoginProvider, string Name) id, CancellationToken cancellationToken = default);
    }
}