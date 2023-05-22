using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Standard.AspNetCore.TestApplication.Domain.Entities;
using Standard.AspNetCore.TestApplication.Domain.Repositories;
using Standard.AspNetCore.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassARepository : RepositoryBase<ClassA, ClassA, ApplicationDbContext>, IClassARepository
    {
        public ClassARepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ClassA> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken) ?? throw new Exception("Id not found.");
        }

        public async Task<List<ClassA>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}