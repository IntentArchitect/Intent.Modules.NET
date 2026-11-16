using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IViewRepository : IEFRepository<View, View>
    {
        [IntentManaged(Mode.Fully)]
        Task<View?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<View?> FindByIdAsync(Guid id, Func<IQueryable<View>, IQueryable<View>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<View>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}