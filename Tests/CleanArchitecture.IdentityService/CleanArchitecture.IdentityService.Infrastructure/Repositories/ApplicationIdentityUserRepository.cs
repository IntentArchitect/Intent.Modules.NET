using AutoMapper;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Domain.Repositories;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ApplicationIdentityUserRepository : RepositoryBase<ApplicationIdentityUser, ApplicationIdentityUser, ApplicationDbContext>, IApplicationIdentityUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationIdentityUserRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public void Add(ApplicationIdentityUser entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO ApplicationIdentityUsers (RefreshToken, RefreshTokenExpired) VALUES({entity.RefreshToken}, {entity.RefreshTokenExpired})");
        }
    }
}