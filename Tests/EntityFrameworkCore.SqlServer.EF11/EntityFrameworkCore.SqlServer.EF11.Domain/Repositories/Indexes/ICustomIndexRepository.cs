using EntityFrameworkCore.SqlServer.EF11.Domain.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomIndexRepository : IEFRepository<CustomIndex, CustomIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<CustomIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CustomIndex?> FindByIdAsync(Guid id, Func<IQueryable<CustomIndex>, IQueryable<CustomIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}