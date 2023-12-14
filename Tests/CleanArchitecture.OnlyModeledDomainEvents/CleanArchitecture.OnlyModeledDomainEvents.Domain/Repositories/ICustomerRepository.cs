using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : ICosmosDBRepository<Customer, ICustomerDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Customer?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}