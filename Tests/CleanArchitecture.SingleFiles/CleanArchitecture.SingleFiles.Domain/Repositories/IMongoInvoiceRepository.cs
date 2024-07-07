using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMongoInvoiceRepository : IMongoRepository<MongoInvoice>
    {
        [IntentManaged(Mode.Fully)]
        List<MongoInvoice> SearchText(string searchText, Expression<Func<MongoInvoice, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(MongoInvoice entity);
        [IntentManaged(Mode.Fully)]
        Task<MongoInvoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MongoInvoice>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}