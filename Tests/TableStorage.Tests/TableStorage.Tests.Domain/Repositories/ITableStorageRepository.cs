using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageRepositoryInterface", Version = "1.0")]

namespace TableStorage.Tests.Domain.Repositories
{
    public interface ITableStorageRepository<TDomain, TTableInterface> : IRepository<TDomain>
    {
        ITableStorageUnitOfWork UnitOfWork { get; }
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<TDomain?> FindByIdAsync((string partitionKey, string rowKey) id, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Expression<Func<TTableInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TTableInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<ICursorPagedList<TDomain>> FindAllAsync(Expression<Func<TTableInterface, bool>> filterExpression, int pageSize, string? cursorToken, CancellationToken cancellationToken = default);
        Task<ICursorPagedList<TDomain>> FindAllAsync(int pageSize, string? cursorToken, CancellationToken cancellationToken = default);
    }
}