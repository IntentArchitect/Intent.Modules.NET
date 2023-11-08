using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Infrastructure.Repositories.Compositional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OneToManySourceRepository : RepositoryBase<OneToManySource, OneToManySource, ApplicationDbContext>, IOneToManySourceRepository
    {
        public OneToManySourceRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<OneToManySource?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<OneToManySource>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}