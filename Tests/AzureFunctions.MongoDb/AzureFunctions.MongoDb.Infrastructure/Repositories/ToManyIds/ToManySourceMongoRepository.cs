using System;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.ToManyIds;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories.ToManyIds
{
    internal class ToManySourceMongoRepository : MongoRepositoryBase<ToManySource, ToManySourceDocument, IToManySourceDocument, string>, IToManySourceRepository
    {
        public ToManySourceMongoRepository(IMongoCollection<ToManySourceDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}