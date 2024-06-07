using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Domain.Contracts;
using Solace.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Solace.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : IEFRepository<Customer, Customer>
    {
        Task<List<Customer>> SearchDapperAsync(CancellationToken cancellationToken = default);
        Task<List<Customer>> SearchSqlEFAsync(CancellationToken cancellationToken = default);
        Task<List<CustomerCustom>> SearchCustomResultAsync(CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Customer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}