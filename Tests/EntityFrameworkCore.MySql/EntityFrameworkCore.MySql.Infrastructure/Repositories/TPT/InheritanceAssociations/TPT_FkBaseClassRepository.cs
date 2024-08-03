using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPT_FkBaseClassRepository : RepositoryBase<TPT_FkBaseClass, TPT_FkBaseClass, ApplicationDbContext>, ITPT_FkBaseClassRepository
    {
        public TPT_FkBaseClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPT_FkBaseClass?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, cancellationToken);
        }
    }
}