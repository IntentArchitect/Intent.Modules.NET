using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.Inheritance
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BaseClassRepository : RepositoryBase<BaseClass, BaseClass, ApplicationDbContext>, IBaseClassRepository
    {
        public BaseClassRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<BaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<BaseClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}