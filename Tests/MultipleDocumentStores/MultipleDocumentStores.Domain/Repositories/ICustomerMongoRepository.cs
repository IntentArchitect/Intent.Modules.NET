using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerMongoRepository : IMongoRepository<CustomerMongo>
    {
        [IntentManaged(Mode.Fully)]
        List<CustomerMongo> SearchText(string searchText, Expression<Func<CustomerMongo, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(CustomerMongo entity);
        [IntentManaged(Mode.Fully)]
        Task<CustomerMongo?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomerMongo>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}