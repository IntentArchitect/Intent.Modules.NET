using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITPT_DerivedClassForConcreteRepository : IEFRepository<TPT_DerivedClassForConcrete, TPT_DerivedClassForConcrete>
    {
        [IntentManaged(Mode.Fully)]
        Task<TPT_DerivedClassForConcrete?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TPT_DerivedClassForConcrete?> FindByIdAsync(Guid id, Func<IQueryable<TPT_DerivedClassForConcrete>, IQueryable<TPT_DerivedClassForConcrete>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TPT_DerivedClassForConcrete>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}