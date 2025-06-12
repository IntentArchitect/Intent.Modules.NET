using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Collections.FolderCollection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Collections.FolderCollection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderCollectionEntityBRepository : IMongoRepository<FolderCollectionEntityB>
    {
        [IntentManaged(Mode.Fully)]
        List<FolderCollectionEntityB> SearchText(string searchText, Expression<Func<FolderCollectionEntityB, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(FolderCollectionEntityB entity);
        [IntentManaged(Mode.Fully)]
        Task<FolderCollectionEntityB?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FolderCollectionEntityB>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}