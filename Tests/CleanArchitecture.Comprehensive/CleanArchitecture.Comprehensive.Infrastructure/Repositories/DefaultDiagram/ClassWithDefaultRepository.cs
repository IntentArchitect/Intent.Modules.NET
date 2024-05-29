using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.DefaultDiagram;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.DefaultDiagram;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.DefaultDiagram
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassWithDefaultRepository : RepositoryBase<ClassWithDefault, ClassWithDefault, ApplicationDbContext>, IClassWithDefaultRepository
    {
        public ClassWithDefaultRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<ClassWithDefault?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassWithDefault>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}