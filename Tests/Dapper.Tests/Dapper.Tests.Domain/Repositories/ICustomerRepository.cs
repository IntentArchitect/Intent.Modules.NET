using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapper.EntityRepositoryInterface", Version = "1.0")]

namespace Dapper.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : IDapperRepository<Customer>
    {
        Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        List<Customer> SearchCustomer(string searchTerm);
    }
}