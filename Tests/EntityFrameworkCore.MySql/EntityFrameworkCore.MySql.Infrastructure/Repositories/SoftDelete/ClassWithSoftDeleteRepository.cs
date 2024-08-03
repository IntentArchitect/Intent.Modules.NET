using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.SoftDelete;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.SoftDelete;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.SoftDelete
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassWithSoftDeleteRepository : RepositoryBase<ClassWithSoftDelete, ClassWithSoftDelete, ApplicationDbContext>, IClassWithSoftDeleteRepository
    {
        public ClassWithSoftDeleteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ClassWithSoftDelete?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassWithSoftDelete>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}