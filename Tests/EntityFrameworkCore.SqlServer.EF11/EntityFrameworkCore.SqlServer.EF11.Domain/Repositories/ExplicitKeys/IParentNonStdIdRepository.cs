using EntityFrameworkCore.SqlServer.EF11.Domain.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IParentNonStdIdRepository : IEFRepository<ParentNonStdId, ParentNonStdId>
    {
        [IntentManaged(Mode.Fully)]
        Task<ParentNonStdId?> FindByIdAsync(Guid myId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ParentNonStdId?> FindByIdAsync(Guid myId, Func<IQueryable<ParentNonStdId>, IQueryable<ParentNonStdId>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ParentNonStdId>> FindByIdsAsync(Guid[] myIds, CancellationToken cancellationToken = default);
    }
}