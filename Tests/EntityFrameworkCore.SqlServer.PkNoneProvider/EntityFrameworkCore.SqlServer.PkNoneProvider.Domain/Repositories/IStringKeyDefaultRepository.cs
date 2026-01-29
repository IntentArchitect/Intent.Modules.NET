using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IStringKeyDefaultRepository : IEFRepository<StringKeyDefault, StringKeyDefault>
    {
        [IntentManaged(Mode.Fully)]
        Task<StringKeyDefault?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<StringKeyDefault?> FindByIdAsync(string id, Func<IQueryable<StringKeyDefault>, IQueryable<StringKeyDefault>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<StringKeyDefault>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}