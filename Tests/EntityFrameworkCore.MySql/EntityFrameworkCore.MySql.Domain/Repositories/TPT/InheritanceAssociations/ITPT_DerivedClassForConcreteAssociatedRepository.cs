using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_DerivedClassForConcreteAssociatedRepository : IEFRepository<TPT_DerivedClassForConcreteAssociated, TPT_DerivedClassForConcreteAssociated>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_DerivedClassForConcreteAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_DerivedClassForConcreteAssociated>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}