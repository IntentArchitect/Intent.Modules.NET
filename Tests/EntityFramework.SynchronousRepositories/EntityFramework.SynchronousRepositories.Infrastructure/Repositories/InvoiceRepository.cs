using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFramework.SynchronousRepositories.Domain.Entities;
using EntityFramework.SynchronousRepositories.Domain.Repositories;
using EntityFramework.SynchronousRepositories.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFramework.SynchronousRepositories.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InvoiceRepository : RepositoryBase<Invoice, Invoice, ApplicationDbContext>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public TProjection? FindByIdProjectTo<TProjection>(Guid id)
        {
            return FindProjectTo<TProjection>(x => x.Id == id);
        }

        public async Task<Invoice?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Invoice?> FindByIdAsync(
            Guid id,
            Func<IQueryable<Invoice>, IQueryable<Invoice>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Invoice>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }

        [IntentManaged(Mode.Fully)]
        public Invoice? FindById(Guid id)
        {
            return Find(x => x.Id == id);
        }

        [IntentManaged(Mode.Fully)]
        public Invoice? FindById(Guid id, Func<IQueryable<Invoice>, IQueryable<Invoice>> queryOptions)
        {
            return Find(x => x.Id == id, queryOptions);
        }

        [IntentManaged(Mode.Fully)]
        public List<Invoice> FindByIds(Guid[] ids)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return FindAll(x => idList.Contains(x.Id));
        }
    }
}