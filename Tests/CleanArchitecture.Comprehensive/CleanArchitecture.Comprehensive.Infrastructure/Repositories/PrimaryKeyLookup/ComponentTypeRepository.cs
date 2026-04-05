using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.PrimaryKeyLookup;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using CleanArchitecture.Comprehensive.Domain.Repositories.PrimaryKeyLookup;
using CleanArchitecture.Comprehensive.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.Repositories.PrimaryKeyLookup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ComponentTypeRepository : RepositoryBase<ComponentType, ComponentType, ApplicationDbContext>, IComponentTypeRepository
    {
        public ComponentTypeRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int componentTypeId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.ComponentTypeId == componentTypeId, cancellationToken);
        }

        public async Task<ComponentType?> FindByIdAsync(int componentTypeId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.ComponentTypeId == componentTypeId, cancellationToken);
        }

        public async Task<ComponentType?> FindByIdAsync(
            int componentTypeId,
            Func<IQueryable<ComponentType>, IQueryable<ComponentType>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.ComponentTypeId == componentTypeId, queryOptions, cancellationToken);
        }

        public async Task<List<ComponentType>> FindByIdsAsync(
            int[] componentTypeIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = componentTypeIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.ComponentTypeId), cancellationToken);
        }
    }
}