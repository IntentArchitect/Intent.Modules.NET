using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomer2CTRepository : IEFRepository<Customer2CT, Customer2CT>
    {
        [IntentManaged(Mode.Fully)]
        Task<Customer2CT?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Customer2CT>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}