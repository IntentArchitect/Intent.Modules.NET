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
    public class CustomPkRepository : RepositoryBase<CustomPk, CustomPk, ApplicationDbContext>, ICustomPkRepository
    {
        public CustomPkRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid myId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.MyId == myId, cancellationToken);
        }

        public async Task<CustomPk?> FindByIdAsync(Guid myId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.MyId == myId, cancellationToken);
        }

        public async Task<List<CustomPk>> FindByIdsAsync(Guid[] myIds, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => myIds.Contains(x.MyId), cancellationToken);
        }
    }
}