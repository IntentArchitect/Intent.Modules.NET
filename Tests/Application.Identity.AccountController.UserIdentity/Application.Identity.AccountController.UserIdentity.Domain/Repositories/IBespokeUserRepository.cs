using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Identity.AccountController.UserIdentity.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBespokeUserRepository : IEFRepository<BespokeUser, BespokeUser>
    {
        [IntentManaged(Mode.Fully)]
        Task<BespokeUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<BespokeUser>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}