using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Accounts.NotSchema;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.Accounts.NotSchema;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.Accounts.NotSchema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AccViewFolderRepository : RepositoryBase<AccViewFolder, AccViewFolder, ApplicationDbContext>, IAccViewFolderRepository
    {
        public AccViewFolderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<AccViewFolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AccViewFolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}