using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Kafka.Consumer.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IInvoiceRepository : IEFRepository<Invoice, Invoice>
    {
        [IntentManaged(Mode.Fully)]
        Task<Invoice?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Invoice>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}