using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_DerivedClassForAbstractAssociatedRepository : IRepository<TPT_DerivedClassForAbstractAssociated, TPT_DerivedClassForAbstractAssociated>
    {

        [IntentManaged(Mode.Fully)]
        Task<TPT_DerivedClassForAbstractAssociated> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_DerivedClassForAbstractAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}