using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.PrimaryKeyTypes;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories.PrimaryKeyTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NewClassLongRepository : RepositoryBase<NewClassLong, NewClassLong, ApplicationDbContext>, INewClassLongRepository
    {
        public NewClassLongRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            long id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<NewClassLong?> FindByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<NewClassLong>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}