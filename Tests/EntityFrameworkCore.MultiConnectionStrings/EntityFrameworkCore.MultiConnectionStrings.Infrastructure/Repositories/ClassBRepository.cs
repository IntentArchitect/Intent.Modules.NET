using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiConnectionStrings.Domain.Entities;
using EntityFrameworkCore.MultiConnectionStrings.Domain.Repositories;
using EntityFrameworkCore.MultiConnectionStrings.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MultiConnectionStrings.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassBRepository : RepositoryBase<ClassB, ClassB, AlternateDbDbContext>, IClassBRepository
    {
        public ClassBRepository(AlternateDbDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<ClassB?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassB>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}