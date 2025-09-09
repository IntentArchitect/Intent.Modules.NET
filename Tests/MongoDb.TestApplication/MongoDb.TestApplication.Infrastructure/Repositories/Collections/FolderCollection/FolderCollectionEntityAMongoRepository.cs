using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Collections.FolderCollection;
using MongoDb.TestApplication.Domain.Repositories.Collections.FolderCollection;
using MongoDb.TestApplication.Domain.Repositories.Documents.Collections.FolderCollection;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections.FolderCollection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Collections.FolderCollection
{
    internal class FolderCollectionEntityAMongoRepository : MongoRepositoryBase<FolderCollectionEntityA, FolderCollectionEntityADocument, IFolderCollectionEntityADocument, string>, IFolderCollectionEntityARepository
    {
        public FolderCollectionEntityAMongoRepository(IMongoCollection<FolderCollectionEntityADocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}