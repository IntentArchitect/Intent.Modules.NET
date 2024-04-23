using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_DerivedClassForAbstractAssociatedRepository : IEFRepository<TPT_DerivedClassForAbstractAssociated, TPT_DerivedClassForAbstractAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_DerivedClassForAbstractAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_DerivedClassForAbstractAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}