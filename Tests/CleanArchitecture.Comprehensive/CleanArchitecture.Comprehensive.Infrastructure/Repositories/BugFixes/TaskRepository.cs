using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.BugFixes;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.BugFixes;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.BugFixes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TaskRepository : RepositoryBase<Domain.Entities.BugFixes.Task, Domain.Entities.BugFixes.Task, ApplicationDbContext>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Domain.Entities.BugFixes.Task?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Domain.Entities.BugFixes.Task>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}