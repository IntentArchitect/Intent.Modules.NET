using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.SynchronousRepositories.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface", Version = "1.0")]

namespace EntityFramework.SynchronousRepositories.Domain.Repositories
{
    public interface IEFRepository<TDomain, TPersistence> : IRepository<TDomain>
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default);
        Task<IPagedResult<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        TDomain? Find(Expression<Func<TPersistence, bool>> filterExpression);
        TDomain? Find(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq);
        List<TDomain> FindAll();
        List<TDomain> FindAll(Expression<Func<TPersistence, bool>> filterExpression);
        List<TDomain> FindAll(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq);
        IPagedResult<TDomain> FindAll(int pageNo, int pageSize);
        IPagedResult<TDomain> FindAll(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize);
        IPagedResult<TDomain> FindAll(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq);
        int Count(Expression<Func<TPersistence, bool>> filterExpression);
        bool Any(Expression<Func<TPersistence, bool>> filterExpression);
        Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>>? filterExpression, CancellationToken cancellationToken = default);
        Task<TProjection?> FindProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>>? filterExpression, CancellationToken cancellationToken = default);
        List<TProjection> FindAllProjectTo<TProjection>(Expression<Func<TPersistence, bool>>? filterExpression);
        TProjection? FindProjectTo<TProjection>(Expression<Func<TPersistence, bool>>? filterExpression);
    }
}