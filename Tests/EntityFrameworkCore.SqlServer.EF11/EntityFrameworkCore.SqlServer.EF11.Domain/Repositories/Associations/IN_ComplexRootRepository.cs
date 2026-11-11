using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IN_ComplexRootRepository : IEFRepository<N_ComplexRoot, N_ComplexRoot>
    {
        [IntentManaged(Mode.Fully)]
        Task<N_ComplexRoot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<N_ComplexRoot?> FindByIdAsync(Guid id, Func<IQueryable<N_ComplexRoot>, IQueryable<N_ComplexRoot>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<N_ComplexRoot>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}