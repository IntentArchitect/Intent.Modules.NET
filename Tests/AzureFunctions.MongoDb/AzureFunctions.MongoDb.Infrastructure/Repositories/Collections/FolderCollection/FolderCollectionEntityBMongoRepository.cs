using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Collections.FolderCollection;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Collections.FolderCollection;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Collections.FolderCollection;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Collections.FolderCollection;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Collections.FolderCollection
{
    internal class FolderCollectionEntityBMongoRepository : MongoRepositoryBase<FolderCollectionEntityB, FolderCollectionEntityBDocument, IFolderCollectionEntityBDocument, string>, IFolderCollectionEntityBRepository
    {
        public FolderCollectionEntityBMongoRepository(IMongoCollection<FolderCollectionEntityBDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}