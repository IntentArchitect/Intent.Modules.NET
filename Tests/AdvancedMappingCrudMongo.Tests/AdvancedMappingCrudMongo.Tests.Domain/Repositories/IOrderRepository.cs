using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOrderRepository : IMongoRepository<Order>
    {
        [IntentManaged(Mode.Fully)]
        List<Order> SearchText(string searchText, Expression<Func<Order, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(Order entity);
        [IntentManaged(Mode.Fully)]
        Task<Order?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Order>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}