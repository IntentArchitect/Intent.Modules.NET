using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPH_MiddleAbstract_RootRepository : RepositoryBase<TPH_MiddleAbstract_Root, TPH_MiddleAbstract_Root, ApplicationDbContext>, ITPH_MiddleAbstract_RootRepository
    {
        public TPH_MiddleAbstract_RootRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TPH_MiddleAbstract_Root?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<TPH_MiddleAbstract_Root>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}