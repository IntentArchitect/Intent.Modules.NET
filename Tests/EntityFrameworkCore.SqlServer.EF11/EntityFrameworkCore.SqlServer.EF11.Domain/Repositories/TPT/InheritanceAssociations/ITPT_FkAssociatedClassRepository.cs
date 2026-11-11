using EntityFrameworkCore.SqlServer.EF11.Domain.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_FkAssociatedClassRepository : IEFRepository<TPT_FkAssociatedClass, TPT_FkAssociatedClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_FkAssociatedClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPT_FkAssociatedClass?> FindByIdAsync(Guid id, Func<IQueryable<TPT_FkAssociatedClass>, IQueryable<TPT_FkAssociatedClass>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_FkAssociatedClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}