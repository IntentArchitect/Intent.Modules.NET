using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.BasicAudit;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.BasicAudit;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.BasicAudit
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Audit_DerivedClassRepository : RepositoryBase<Audit_DerivedClass, Audit_DerivedClass, ApplicationDbContext>, IAudit_DerivedClassRepository
    {
        public Audit_DerivedClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Audit_DerivedClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Audit_DerivedClass>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}