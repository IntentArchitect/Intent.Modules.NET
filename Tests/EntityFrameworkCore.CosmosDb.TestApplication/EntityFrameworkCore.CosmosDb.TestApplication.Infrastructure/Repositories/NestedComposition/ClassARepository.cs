using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.NestedComposition;
using EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Infrastructure.Repositories.NestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassARepository : RepositoryBase<ClassA, ClassA, ApplicationDbContext>, IClassARepository
    {
        public ClassARepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<ClassA?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassA>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}