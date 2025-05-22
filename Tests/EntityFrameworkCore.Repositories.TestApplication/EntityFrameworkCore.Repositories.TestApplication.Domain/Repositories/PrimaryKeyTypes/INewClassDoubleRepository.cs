using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.PrimaryKeyTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.PrimaryKeyTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INewClassDoubleRepository : IEFRepository<NewClassDouble, NewClassDouble>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(double id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NewClassDouble?> FindByIdAsync(double id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<NewClassDouble?> FindByIdAsync(double id, Func<IQueryable<NewClassDouble>, IQueryable<NewClassDouble>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NewClassDouble>> FindByIdsAsync(double[] ids, CancellationToken cancellationToken = default);
    }
}