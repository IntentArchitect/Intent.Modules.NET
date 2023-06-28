using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerAnemicRepository : IEFRepository<CustomerAnemic, CustomerAnemic>
    {
        [IntentManaged(Mode.Fully)]
        Task<CustomerAnemic?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomerAnemic>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}