using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapper.RepositoryInterface", Version = "1.0")]

namespace Dapper.Tests.Domain.Repositories
{
    public interface IDapperRepository<TDomain>
    {
        Task AddAsync(TDomain entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TDomain entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(TDomain entity, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    }
}