using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPC_DerivedClassForAbstractAssociatedRepository : IEFRepository<TPC_DerivedClassForAbstractAssociated, TPC_DerivedClassForAbstractAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPC_DerivedClassForAbstractAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPC_DerivedClassForAbstractAssociated?> FindByIdAsync(Guid id, Func<IQueryable<TPC_DerivedClassForAbstractAssociated>, IQueryable<TPC_DerivedClassForAbstractAssociated>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPC_DerivedClassForAbstractAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}