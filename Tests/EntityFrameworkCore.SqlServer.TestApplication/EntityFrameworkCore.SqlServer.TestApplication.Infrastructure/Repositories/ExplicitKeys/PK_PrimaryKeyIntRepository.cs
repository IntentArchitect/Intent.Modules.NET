using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PK_PrimaryKeyIntRepository : RepositoryBase<PK_PrimaryKeyInt, PK_PrimaryKeyInt, ApplicationDbContext>, IPK_PrimaryKeyIntRepository
    {
        public PK_PrimaryKeyIntRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<PK_PrimaryKeyInt?> FindByIdAsync(int primaryKeyId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PrimaryKeyId == primaryKeyId, cancellationToken);
        }

        public async Task<List<PK_PrimaryKeyInt>> FindByIdsAsync(
            int[] primaryKeyIds,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => primaryKeyIds.Contains(x.PrimaryKeyId), cancellationToken);
        }
    }
}