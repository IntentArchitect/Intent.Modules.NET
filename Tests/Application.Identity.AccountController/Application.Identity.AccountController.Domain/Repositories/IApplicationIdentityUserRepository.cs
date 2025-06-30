using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Identity.AccountController.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Application.Identity.AccountController.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IApplicationIdentityUserRepository : IEFRepository<ApplicationIdentityUser, ApplicationIdentityUser>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ApplicationIdentityUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ApplicationIdentityUser?> FindByIdAsync(string id, Func<IQueryable<ApplicationIdentityUser>, IQueryable<ApplicationIdentityUser>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ApplicationIdentityUser>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}