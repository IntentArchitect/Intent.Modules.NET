using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IJ_MultipleDependentRepository : IRepository<J_MultipleDependent, J_MultipleDependent>
    {
        [IntentManaged(Mode.Fully)]
        Task<J_MultipleDependent> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<J_MultipleDependent>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}