using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IH_MultipleDependentRepository : IMongoRepository<H_MultipleDependent>
    {
        [IntentManaged(Mode.Fully)]
        List<H_MultipleDependent> SearchText(string searchText, Expression<Func<H_MultipleDependent, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(H_MultipleDependent entity);
        [IntentManaged(Mode.Fully)]
        Task<H_MultipleDependent?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<H_MultipleDependent>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}