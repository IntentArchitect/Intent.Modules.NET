using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Application.Common.Pagination;
using Ardalis.Domain.Common.Interfaces;
using Ardalis.Domain.Repositories;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase", Version = "1.0")]

namespace Ardalis.Infrastructure.Repositories
{
    public class RepositoryBase<TDomain, TDbContext> : RepositoryBase<TDomain>, IRepositoryBase<TDomain>
        where TDbContext : DbContext, IUnitOfWork
        where TDomain : class
    {
        protected readonly TDbContext _dbContext;
        private readonly IMapper _mapper;

        public RepositoryBase(TDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork => _dbContext;

        [IntentManaged(Mode.Fully)]
        public void Add(TDomain entity)
        {
            base.AddAsync(entity).Wait();
        }

        [IntentManaged(Mode.Fully)]
        public void Remove(TDomain entity)
        {
            base.DeleteAsync(entity).Wait();
        }

        [IntentManaged(Mode.Fully)]
        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return (await ListAsync(cancellationToken)).Cast<TDomain>().ToList();
        }

        [IntentManaged(Mode.Fully)]
        public Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return FindAllAsync(filterExpression: x => true, pageNo: pageNo, pageSize: pageSize, cancellationToken: cancellationToken);
        }

        [IntentManaged(Mode.Fully)]
        public Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return FindAllAsync(filterExpression: filterExpression, pageNo: pageNo, pageSize: pageSize, linq: x => x, cancellationToken: cancellationToken);
        }

        [IntentManaged(Mode.Fully)]
        public async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TDomain> queryable = _dbContext.Set<TDomain>();
            queryable = queryable.Where(filterExpression);
            var result = linq(queryable);
            return await ToPagedListAsync<TDomain>(
                result,
                pageNo,
                pageSize,
                cancellationToken);
        }

        // To avoid escalating db locks early, we are not going to perform saving changes on every operation.
        // The overall infrastructure will ensure that the DbContext SaveChanges is ultimately invoked.
        [IntentManaged(Mode.Fully)]
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        // In the event that SaveChanges is invoked explicitly, it should still operate as intended.
        [IntentManaged(Mode.Fully)]
        Task<int> IRepositoryBase<TDomain>.SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task<IPagedList<TDomain>> FindAllAsync(
            ISpecification<TDomain> specification,
            int pageNo,
            int pageSize,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = ApplySpecification(specification);
            queryable = queryOptions == null ? queryable : queryOptions(queryable);
            return await ToPagedListAsync<TDomain>(
                            queryable,
                            pageNo,
                            pageSize,
                            cancellationToken);
        }

        public async Task<IPagedList<TDomain>> FindAllAsync(
            ISpecification<TDomain> specification,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(specification, pageNo, pageSize, null, cancellationToken);
        }

        private static async Task<IPagedList<T>> ToPagedListAsync<T>(
            IQueryable<T> queryable,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await queryable.CountAsync(cancellationToken);
            var skip = ((pageNo - 1) * pageSize);

            var results = await queryable
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return new PagedList<T>(count, pageNo, pageSize, results);
        }
    }
}