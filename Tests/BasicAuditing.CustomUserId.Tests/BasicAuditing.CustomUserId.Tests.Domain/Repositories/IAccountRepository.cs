using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BasicAuditing.CustomUserId.Tests.Domain.Entities;
using BasicAuditing.CustomUserId.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccountRepository : ICosmosDBRepository<Account, IAccountDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Account?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}