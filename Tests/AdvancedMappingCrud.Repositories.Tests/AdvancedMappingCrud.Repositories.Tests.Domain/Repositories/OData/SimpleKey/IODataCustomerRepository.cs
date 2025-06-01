using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OData.SimpleKey;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.OData.SimpleKey
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IODataCustomerRepository : IEFRepository<ODataCustomer, ODataCustomer>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ODataCustomer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ODataCustomer?> FindByIdAsync(Guid id, Func<IQueryable<ODataCustomer>, IQueryable<ODataCustomer>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ODataCustomer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}