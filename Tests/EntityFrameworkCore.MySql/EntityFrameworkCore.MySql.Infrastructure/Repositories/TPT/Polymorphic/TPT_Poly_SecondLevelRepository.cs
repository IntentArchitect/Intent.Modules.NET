using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPT.Polymorphic;
using EntityFrameworkCore.MySql.Domain.Repositories;
using EntityFrameworkCore.MySql.Domain.Repositories.TPT.Polymorphic;
using EntityFrameworkCore.MySql.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPT_Poly_SecondLevelRepository : RepositoryBase<TPT_Poly_SecondLevel, TPT_Poly_SecondLevel, ApplicationDbContext>, ITPT_Poly_SecondLevelRepository
    {
        public TPT_Poly_SecondLevelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPT_Poly_SecondLevel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<TPT_Poly_SecondLevel>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}