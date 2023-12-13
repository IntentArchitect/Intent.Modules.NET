using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Entities;
using MultipleDocumentStores.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerCosmosRepository : ICosmosDBRepository<CustomerCosmos, ICustomerCosmosDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<CustomerCosmos?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}