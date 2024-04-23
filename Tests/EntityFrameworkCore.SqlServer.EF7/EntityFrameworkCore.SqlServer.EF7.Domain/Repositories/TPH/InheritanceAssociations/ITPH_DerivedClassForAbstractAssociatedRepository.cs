using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPH_DerivedClassForAbstractAssociatedRepository : IEFRepository<TPH_DerivedClassForAbstractAssociated, TPH_DerivedClassForAbstractAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPH_DerivedClassForAbstractAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPH_DerivedClassForAbstractAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}