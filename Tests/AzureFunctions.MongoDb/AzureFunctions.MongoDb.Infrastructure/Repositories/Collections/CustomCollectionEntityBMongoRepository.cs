using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Collections;
using AzureFunctions.MongoDb.Domain.Repositories.Collections;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Collections;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Collections;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Collections
{
    internal class CustomCollectionEntityBMongoRepository : MongoRepositoryBase<CustomCollectionEntityB, CustomCollectionEntityBDocument, ICustomCollectionEntityBDocument, string>, ICustomCollectionEntityBRepository
    {
        public CustomCollectionEntityBMongoRepository(IMongoCollection<CustomCollectionEntityBDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}