using EntityFrameworkCore.SqlServer.EF11.Domain.TPH.Polymorphic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_Poly_ConcreteARepository : IEFRepository<TPH_Poly_ConcreteA, TPH_Poly_ConcreteA>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_ConcreteA?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_Poly_ConcreteA?> FindByIdAsync(Guid id, Func<IQueryable<TPH_Poly_ConcreteA>, IQueryable<TPH_Poly_ConcreteA>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_Poly_ConcreteA>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}