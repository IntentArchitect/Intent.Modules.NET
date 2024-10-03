using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Domain.Common.Interfaces;
using Ardalis.Specification;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface", Version = "1.0")]

namespace Ardalis.Domain.Repositories
{
    public interface IEFRepository<TDomain>
    {
        IUnitOfWork UnitOfWork { get; }
        void Add(TDomain entity);
        void Remove(TDomain entity);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int PageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDomain, bool>> filterExpression, int pageNo, int PageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDomain, bool>> filterExpression, int pageNo, int PageSize, Func<IQueryable<TDomain>, IQueryable<TDomain>> linq, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(ISpecification<TDomain> specification, int pageNo, int PageSize, Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(ISpecification<TDomain> specification, int pageNo, int PageSize, CancellationToken cancellationToken = default);
    }
}