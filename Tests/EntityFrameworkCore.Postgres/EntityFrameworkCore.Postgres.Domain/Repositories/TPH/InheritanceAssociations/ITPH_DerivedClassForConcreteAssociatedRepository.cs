using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_DerivedClassForConcreteAssociatedRepository : IEFRepository<TPH_DerivedClassForConcreteAssociated, TPH_DerivedClassForConcreteAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_DerivedClassForConcreteAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPH_DerivedClassForConcreteAssociated?> FindByIdAsync(Guid id, Func<IQueryable<TPH_DerivedClassForConcreteAssociated>, IQueryable<TPH_DerivedClassForConcreteAssociated>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_DerivedClassForConcreteAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}