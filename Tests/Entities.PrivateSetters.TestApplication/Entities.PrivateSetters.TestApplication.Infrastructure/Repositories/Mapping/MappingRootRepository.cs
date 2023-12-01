using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping;
using Entities.PrivateSetters.TestApplication.Domain.Repositories;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Mapping;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Repositories.Mapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MappingRootRepository : RepositoryBase<MappingRoot, MappingRoot, ApplicationDbContext>, IMappingRootRepository
    {
        public MappingRootRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<MappingRoot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<MappingRoot>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}