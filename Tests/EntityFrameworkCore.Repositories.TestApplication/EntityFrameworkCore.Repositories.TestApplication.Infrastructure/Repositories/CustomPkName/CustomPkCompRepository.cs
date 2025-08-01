using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.CustomPkName;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.CustomPkName;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.CustomPkName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomPkCompRepository : RepositoryBase<CustomPkComp, CustomPkComp, ApplicationDbContext>, ICustomPkCompRepository
    {
        public CustomPkCompRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (Guid MyId, string MyId2) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.MyId == id.MyId && x.MyId2 == id.MyId2, cancellationToken);
        }

        public async Task<CustomPkComp?> FindByIdAsync(
            (Guid MyId, string MyId2) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.MyId == id.MyId && x.MyId2 == id.MyId2, cancellationToken);
        }

        public async Task<CustomPkComp?> FindByIdAsync(
            (Guid MyId, string MyId2) id,
            Func<IQueryable<CustomPkComp>, IQueryable<CustomPkComp>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.MyId == id.MyId && x.MyId2 == id.MyId2, queryOptions, cancellationToken);
        }
    }
}