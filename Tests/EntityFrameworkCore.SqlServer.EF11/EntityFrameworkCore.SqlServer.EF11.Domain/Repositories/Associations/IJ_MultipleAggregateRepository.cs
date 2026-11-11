using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IJ_MultipleAggregateRepository : IEFRepository<J_MultipleAggregate, J_MultipleAggregate>
    {
        [IntentManaged(Mode.Fully)]
        Task<J_MultipleAggregate?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<J_MultipleAggregate?> FindByIdAsync(Guid id, Func<IQueryable<J_MultipleAggregate>, IQueryable<J_MultipleAggregate>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<J_MultipleAggregate>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}