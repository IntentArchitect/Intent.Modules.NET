using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Domain.Entities.Collections.FolderCollection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Collections.FolderCollection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderCollectionEntityARepository : IMongoRepository<FolderCollectionEntityA>
    {
        [IntentManaged(Mode.Fully)]
        List<FolderCollectionEntityA> SearchText(string searchText, Expression<Func<FolderCollectionEntityA, bool>> filterExpression = null);
        [IntentManaged(Mode.Fully)]
        void Update(FolderCollectionEntityA entity);
        [IntentManaged(Mode.Fully)]
        Task<FolderCollectionEntityA?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FolderCollectionEntityA>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}