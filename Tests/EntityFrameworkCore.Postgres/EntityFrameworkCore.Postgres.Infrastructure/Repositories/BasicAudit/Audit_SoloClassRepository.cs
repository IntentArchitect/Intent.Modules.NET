using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.BasicAudit;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.BasicAudit;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.BasicAudit
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Audit_SoloClassRepository : RepositoryBase<Audit_SoloClass, Audit_SoloClass, ApplicationDbContext>, IAudit_SoloClassRepository
    {
        public Audit_SoloClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Audit_SoloClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Audit_SoloClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}