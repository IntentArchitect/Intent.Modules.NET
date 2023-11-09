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

        public async Task<AggregateRoot5EntityWithRepo?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AggregateRoot5EntityWithRepo>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}