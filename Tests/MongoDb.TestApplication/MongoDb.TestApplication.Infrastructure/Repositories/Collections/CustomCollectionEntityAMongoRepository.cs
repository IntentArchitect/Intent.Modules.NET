using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Collections;
using MongoDb.TestApplication.Domain.Repositories.Collections;
using MongoDb.TestApplication.Domain.Repositories.Documents.Collections;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoDb.TestApplication.Infrastructure.Persistence.Documents.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories.Collections
{
    internal class CustomCollectionEntityAMongoRepository : MongoRepositoryBase<CustomCollectionEntityA, CustomCollectionEntityADocument, ICustomCollectionEntityADocument, string>, ICustomCollectionEntityARepository
    {
        public CustomCollectionEntityAMongoRepository(IMongoCollection<CustomCollectionEntityADocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}