using EntityFrameworkCore.SqlServer.EF11.Domain.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_ConcreteBaseClassRepository : IEFRepository<TPH_ConcreteBaseClass, TPH_ConcreteBaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_ConcreteBaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_ConcreteBaseClass?> FindByIdAsync(Guid id, Func<IQueryable<TPH_ConcreteBaseClass>, IQueryable<TPH_ConcreteBaseClass>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_ConcreteBaseClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}