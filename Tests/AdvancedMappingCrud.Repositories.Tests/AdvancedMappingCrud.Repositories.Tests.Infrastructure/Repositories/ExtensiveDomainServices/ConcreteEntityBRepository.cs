using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.ExtensiveDomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ConcreteEntityBRepository : RepositoryBase<ConcreteEntityB, ConcreteEntityB, ApplicationDbContext>, IConcreteEntityBRepository
    {
        public ConcreteEntityBRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<ConcreteEntityB?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ConcreteEntityB?> FindByIdAsync(
            Guid id,
            Func<IQueryable<ConcreteEntityB>, IQueryable<ConcreteEntityB>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<ConcreteEntityB>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}