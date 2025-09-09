using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes;
using AzureFunctions.MongoDb.Domain.Repositories.Indexes;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Indexes;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.Indexes
{
    internal class SingleIndexEntityMongoRepository : MongoRepositoryBase<SingleIndexEntity, SingleIndexEntityDocument, ISingleIndexEntityDocument, string>, ISingleIndexEntityRepository
    {
        public SingleIndexEntityMongoRepository(IMongoCollection<SingleIndexEntityDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}