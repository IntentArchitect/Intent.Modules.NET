using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Entities;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : ICosmosDBRepository<Customer, ICustomerDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}