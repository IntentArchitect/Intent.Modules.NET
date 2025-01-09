using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Common;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITemporalProductRepository : IEFRepository<TemporalProduct, TemporalProduct>
    {
        [IntentManaged(Mode.Fully)]
        Task<TemporalProduct?> FindByIdAsync(Guid id1, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TemporalProduct>> FindByIdsAsync(Guid[] id1s, CancellationToken cancellationToken = default);
        Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(TemporalHistoryQueryOptions historyOptions, CancellationToken cancellationToken = default);
        Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(TemporalHistoryQueryOptions historyOptions, Expression<Func<TemporalProduct, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(TemporalHistoryQueryOptions historyOptions, Expression<Func<TemporalProduct, bool>> filterExpression, Func<IQueryable<TemporalProduct>, IQueryable<TemporalProduct>> queryOptions, CancellationToken cancellationToken = default);
    }
}