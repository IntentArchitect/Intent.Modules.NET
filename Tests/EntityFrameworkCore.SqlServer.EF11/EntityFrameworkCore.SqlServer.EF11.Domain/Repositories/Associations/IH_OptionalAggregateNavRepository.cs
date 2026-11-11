using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IH_OptionalAggregateNavRepository : IEFRepository<H_OptionalAggregateNav, H_OptionalAggregateNav>
    {
        [IntentManaged(Mode.Fully)]
        Task<H_OptionalAggregateNav?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<H_OptionalAggregateNav?> FindByIdAsync(Guid id, Func<IQueryable<H_OptionalAggregateNav>, IQueryable<H_OptionalAggregateNav>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<H_OptionalAggregateNav>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}