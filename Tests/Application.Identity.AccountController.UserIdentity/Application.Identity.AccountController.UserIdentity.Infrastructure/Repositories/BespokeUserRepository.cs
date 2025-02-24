using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Identity.AccountController.UserIdentity.Domain.Entities;
using Application.Identity.AccountController.UserIdentity.Domain.Repositories;
using Application.Identity.AccountController.UserIdentity.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BespokeUserRepository : RepositoryBase<BespokeUser, BespokeUser, ApplicationDbContext>, IBespokeUserRepository
    {
        public BespokeUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<BespokeUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<BespokeUser>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}