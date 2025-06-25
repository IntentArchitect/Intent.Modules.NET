using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Identity.AccountController.UserIdentity.Domain.Entities;
using Application.Identity.AccountController.UserIdentity.Domain.Repositories;
using Application.Identity.AccountController.UserIdentity.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Application.Identity.AccountController.UserIdentity.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BespokeUserRepository : RepositoryBase<BespokeUser, BespokeUser, ApplicationDbContext>, IBespokeUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BespokeUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(BespokeUser entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO BespokeUsers (RefreshToken, RefreshTokenExpired, FirstName, LastName) VALUES({entity.RefreshToken}, {entity.RefreshTokenExpired}, {entity.FirstName}, {entity.LastName})");
        }

        public async Task<BespokeUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<BespokeUser?> FindByIdAsync(
            string id,
            Func<IQueryable<BespokeUser>, IQueryable<BespokeUser>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<BespokeUser>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}