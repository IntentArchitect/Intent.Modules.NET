using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRoot5EntityWithRepoRepository : RepositoryBase<AggregateRoot5EntityWithRepo, AggregateRoot5EntityWithRepo, ApplicationDbContext>, IAggregateRoot5EntityWithRepoRepository
    {
        public AggregateRoot5EntityWithRepoRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            // The AggregateRoot5EntityWithRepo has no EntityFrameworkCore type configuration associated with it.
            // Add the 'Table' stereotype to this entity in the Domain designer.
            throw new NotSupportedException($"Cannot create a repository for type AggregateRoot5EntityWithRepo.");
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<AggregateRoot5EntityWithRepo?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<AggregateRoot5EntityWithRepo?> FindByIdAsync(
            Guid id,
            Func<IQueryable<AggregateRoot5EntityWithRepo>, IQueryable<AggregateRoot5EntityWithRepo>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<AggregateRoot5EntityWithRepo>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}