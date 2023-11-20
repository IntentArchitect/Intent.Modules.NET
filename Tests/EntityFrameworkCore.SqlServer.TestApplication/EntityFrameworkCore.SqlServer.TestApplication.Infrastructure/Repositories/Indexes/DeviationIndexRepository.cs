using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.Indexes;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeviationIndexRepository : RepositoryBase<DeviationIndex, DeviationIndex, ApplicationDbContext>, IDeviationIndexRepository
    {
        public DeviationIndexRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<DeviationIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<DeviationIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}