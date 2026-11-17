using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IC_RequiredCompositeRepository : IEFRepository<C_RequiredComposite, C_RequiredComposite>
    {
        [IntentManaged(Mode.Fully)]
        Task<C_RequiredComposite?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<C_RequiredComposite?> FindByIdAsync(Guid id, Func<IQueryable<C_RequiredComposite>, IQueryable<C_RequiredComposite>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<C_RequiredComposite>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}