using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IInvoiceRepository : IMongoRepository<Invoice>
    {
        [IntentManaged(Mode.Fully)]
        List<Invoice> SearchText(string searchText, Expression<Func<Invoice, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(Invoice entity);
        [IntentManaged(Mode.Fully)]
        Task<Invoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Invoice>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}