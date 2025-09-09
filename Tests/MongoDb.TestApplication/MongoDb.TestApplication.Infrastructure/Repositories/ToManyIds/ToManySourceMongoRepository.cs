using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.ToManyIds;
using MongoDb.TestApplication.Domain.Repositories.Documents.ToManyIds;
using MongoDb.TestApplication.Domain.Repositories.ToManyIds;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.ToManyIds;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.ToManyIds
{
    internal class ToManySourceMongoRepository : MongoRepositoryBase<ToManySource, ToManySourceDocument, IToManySourceDocument, string>, IToManySourceRepository
    {
        public ToManySourceMongoRepository(IMongoCollection<ToManySourceDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}