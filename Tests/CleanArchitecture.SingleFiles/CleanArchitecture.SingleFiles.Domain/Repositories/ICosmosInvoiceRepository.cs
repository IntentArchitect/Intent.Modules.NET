using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICosmosInvoiceRepository : ICosmosDBRepository<CosmosInvoice, ICosmosInvoiceDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<CosmosInvoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}