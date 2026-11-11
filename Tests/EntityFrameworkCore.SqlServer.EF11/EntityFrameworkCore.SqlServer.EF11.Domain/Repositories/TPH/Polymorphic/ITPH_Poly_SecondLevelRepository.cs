using EntityFrameworkCore.SqlServer.EF11.Domain.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_Poly_SecondLevelRepository : IEFRepository<TPH_Poly_SecondLevel, TPH_Poly_SecondLevel>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_SecondLevel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_SecondLevel?> FindByIdAsync(Guid id, Func<IQueryable<TPH_Poly_SecondLevel>, IQueryable<TPH_Poly_SecondLevel>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_Poly_SecondLevel>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}